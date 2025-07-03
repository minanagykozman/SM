using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.BAL
{
    public class FundHandler : HandlerBase
    {

        // Get all funds with optional filtering (Admin only or assigned funds for Servants)
        public List<MemberFund> GetAllFunds(int? assigneeId, string? status, string username)
        {
            var servant = GetServantByUsername(username);
            var query = _dbcontext.MemberFunds
                .Include(f => f.Member)
                .Include(f => f.Servant)
                .Include(f => f.Approver)
                .AsQueryable();

            // Role-based filtering
            if (!IsAdmin(username))
            {
                // Servants can only see funds assigned to them
                query = query.Where(f => f.ServantID == servant.ServantID);
            }
            else if (assigneeId.HasValue)
            {
                // Admin can filter by assignee
                query = query.Where(f => f.ServantID == assigneeId.Value);
            }

            // Status filtering
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<FundStatus>(status, out var statusEnum))
            {
                query = query.Where(f => f.Status == statusEnum);
            }

            return query.OrderByDescending(f => f.RequestDate).ToList();
        }

        // Get funds grouped by status for workflow board
        public object GetFundsByStatus(int? assigneeId, string? status, string? searchTerm, string username)
        {
            var servant = GetServantByUsername(username);
            var query = _dbcontext.MemberFunds
                .Include(f => f.Member)
                .Include(f => f.Servant)
                .Include(f => f.Approver)
                .AsQueryable();

            // Role-based filtering
            if (!IsAdmin(username))
            {
                query = query.Where(f => f.ServantID == servant.ServantID);
            }
            else if (assigneeId.HasValue)
            {
                query = query.Where(f => f.ServantID == assigneeId.Value);
            }

            // Status filtering
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<FundStatus>(status, out var statusEnum))
            {
                query = query.Where(f => f.Status == statusEnum);
            }

            // Search term filtering
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.Member.FullName.Contains(searchTerm) ||
                                       f.RequestDescription.Contains(searchTerm));
            }

            var funds = query.OrderByDescending(f => f.RequestDate).ToList();

            return new
            {
                Open = funds.Where(f => f.Status == FundStatus.Open).ToList(),
                Approved = funds.Where(f => f.Status == FundStatus.Approved).ToList(),
                Completed = funds.Where(f => f.Status == FundStatus.Delivered || f.Status == FundStatus.Rejected).ToList()
            };
        }

        // Get a specific fund
        public MemberFund? GetFund(int fundId, string username)
        {
            var servant = GetServantByUsername(username);
            var query = _dbcontext.MemberFunds
                .Include(f => f.Member)
                .Include(f => f.Servant)
                .Include(f => f.Approver)
                .Where(f => f.FundID == fundId);

            // Role-based access
            if (!IsAdmin(username))
            {
                query = query.Where(f => f.ServantID == servant.ServantID);
            }

            var fund = query.FirstOrDefault();
            
            if (fund?.Member != null)
            {
                int familyCount = _dbcontext.Members.Count(m => m.UNFileNumber == fund.Member.UNFileNumber);
                fund.Member.FamilyCount = familyCount - 1;
            }

            return fund;
        }

        // Create a new fund
        public int CreateFund(CreateFundRequest request, string username)
        {
            try
            {
                Servant servant = GetServantByUsername(username);
                // Validate assignee exists
                var assignee = _dbcontext.Servants.FirstOrDefault(s => s.ServantID == request.ApproverID);
                if (assignee == null)
                {
                    throw new ArgumentException($"Invalid assignee ID: {request.ApproverID}");
                }

                // Validate member exists
                var member = _dbcontext.Members.FirstOrDefault(m => m.MemberID == request.MemberID);
                if (member == null)
                {
                    throw new ArgumentException($"Invalid member ID: {request.MemberID}");
                }

                var fund = new MemberFund
                {
                    MemberID = request.MemberID,
                    RequestDescription = request.RequestDescription ?? string.Empty,
                    ServantID = servant.ServantID,
                    ApproverID = request.ApproverID,
                    RequestedAmount = request.RequestedAmount,
                    FundCategory = request.FundCategory ?? "Others",
                    Status = FundStatus.Open,
                    RequestDate = CurrentTime,
                    ApproverNotes = request.ApproverNotes ?? string.Empty,
                    ApprovedAmount = 0
                };

                _dbcontext.MemberFunds.Add(fund);
                _dbcontext.SaveChanges();

                return fund.FundID;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating fund: {ex.Message}", ex);
            }
        }

        // Update fund status
        public void UpdateFundStatus(UpdateFundStatusRequest request, string username)
        {
            var fund = GetFund(request.FundID, username);
            if (fund == null)
            {
                throw new ArgumentException("Fund not found or access denied");
            }

            var servant = GetServantByUsername(username);

            // Validate status
            if (!Enum.TryParse<FundStatus>(request.Status, out var statusEnum))
            {
                throw new ArgumentException("Invalid status");
            }

            fund.Status = statusEnum;

            // Set approver if status is being approved/rejected
            if (statusEnum == FundStatus.Approved || statusEnum == FundStatus.Rejected)
            {
                fund.ApproverID = servant.ServantID;
                if (request.ApprovedAmount.HasValue)
                {
                    fund.ApprovedAmount = request.ApprovedAmount.Value;
                }
            }

            // Append notes if provided
            if (!string.IsNullOrEmpty(request.ApproverNotes))
            {
                var timestamp = CurrentTime.ToString("yyyy-MM-dd HH:mm");
                var noteEntry = $"[{timestamp} - {servant.ServantName}]: {request.ApproverNotes}";

                if (string.IsNullOrEmpty(fund.ApproverNotes))
                {
                    fund.ApproverNotes = noteEntry;
                }
                else
                {
                    fund.ApproverNotes += "\n" + noteEntry;
                }
            }

            _dbcontext.SaveChanges();
        }

        // Append notes to a fund
        public void AppendNotes(AppendNotesRequest request, string username)
        {
            var fund = GetFund(request.FundID, username);
            if (fund == null)
            {
                throw new ArgumentException("Fund not found or access denied");
            }

            var servant = GetServantByUsername(username);
            var timestamp = CurrentTime.ToString("yyyy-MM-dd HH:mm");
            var noteEntry = $"[{timestamp} - {servant.ServantName}]: {request.ApproverNotes}";

            if (string.IsNullOrEmpty(fund.ApproverNotes))
            {
                fund.ApproverNotes = noteEntry;
            }
            else
            {
                fund.ApproverNotes += "\n" + noteEntry;
            }

            _dbcontext.SaveChanges();
        }

        // Update fund details
        public void UpdateFund(UpdateFundRequest request, string username)
        {
            var fund = GetFund(request.FundID, username);
            if (fund == null)
            {
                throw new ArgumentException("Fund not found or access denied");
            }

            // Only allow updates if fund is still Open or if user is admin
            if (fund.Status != FundStatus.Open && !IsAdmin(username))
            {
                throw new InvalidOperationException("Cannot update fund that is not in Open status");
            }

            if (!string.IsNullOrEmpty(request.RequestDescription))
            {
                fund.RequestDescription = request.RequestDescription;
            }

            if (request.ServantID.HasValue)
            {
                var assignee = _dbcontext.Servants.FirstOrDefault(s => s.ServantID == request.ServantID.Value);
                if (assignee == null)
                {
                    throw new ArgumentException("Invalid assignee ID");
                }
                fund.ApproverID = request.ServantID.Value;
            }
            
            if (request.RequestedAmount.HasValue)
            {
                fund.RequestedAmount = request.RequestedAmount.Value;
            }

            if (!string.IsNullOrEmpty(request.FundCategory))
            {
                fund.FundCategory = request.FundCategory;
            }

            if (!string.IsNullOrEmpty(request.ApproverNotes))
            {
                var servant = GetServantByUsername(username);
                var timestamp = CurrentTime.ToString("yyyy-MM-dd HH:mm");
                var noteEntry = $"[{timestamp} - {servant.ServantName}]: {request.ApproverNotes}";

                if (string.IsNullOrEmpty(fund.ApproverNotes))
                {
                    fund.ApproverNotes = noteEntry;
                }
                else
                {
                    fund.ApproverNotes += "\n" + noteEntry;
                }
            }

            _dbcontext.SaveChanges();
        }

        // Delete a fund (Admin only, and only if Open)
        public void DeleteFund(int fundId, string username)
        {
            if (!IsAdmin(username))
            {
                throw new UnauthorizedAccessException("Only administrators can delete funds");
            }

            var fund = _dbcontext.MemberFunds.FirstOrDefault(f => f.FundID == fundId);
            if (fund == null)
            {
                throw new ArgumentException("Fund not found");
            }

            if (fund.Status != FundStatus.Open)
            {
                throw new InvalidOperationException("Can only delete funds in Open status");
            }

            _dbcontext.MemberFunds.Remove(fund);
            _dbcontext.SaveChanges();
        }

        // Get list of servants that can be assigned funds
        public List<object> GetAssignableServants()
        {
            var x = _dbcontext.Servants;
            var servants = _dbcontext.Servants
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    ServantID = s.ServantID,
                    ServantName = s.ServantName
                })
                .OrderBy(s => s.ServantName)
                .ToList<object>();

            return servants;
        }
    }

    public class CreateFundRequest
    {
        public int MemberID { get; set; }
        public string RequestDescription { get; set; } = string.Empty;
        public int ApproverID { get; set; }
        public string? ApproverNotes { get; set; }
        public decimal? RequestedAmount { get; set; }
        public string? FundCategory { get; set; }
    }

    public class UpdateFundStatusRequest
    {
        public int FundID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ApproverNotes { get; set; }
        public decimal? ApprovedAmount { get; set; }
    }

    public class AppendNotesRequest
    {
        public int FundID { get; set; }
        public string ApproverNotes { get; set; } = string.Empty;
    }

    public class UpdateFundRequest
    {
        public int FundID { get; set; }
        public string? RequestDescription { get; set; }
        public int? ServantID { get; set; }
        public string? ApproverNotes { get; set; }
        public decimal? RequestedAmount { get; set; }
        public string? FundCategory { get; set; }
    }
}