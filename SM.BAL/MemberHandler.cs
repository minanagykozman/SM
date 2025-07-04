using Microsoft.EntityFrameworkCore;
using SM.DAL.DataModel;
using System.Text.Json;

namespace SM.BAL
{
    public class MemberHandler : HandlerBase
    {
        public List<Member> GetFamilyByUNFileNumber(string unFileNumber)
        {
            var members = _dbcontext.Members.Where(m => m.UNFileNumber == unFileNumber).ToList<Member>();
            return members.OrderBy(m => m.Birthdate).ToList<Member>();
        }
        public void UpdateMemberStatus()
        {
            var activeMembers = _dbcontext.ClassAttendances.Select(c => c.Member).Distinct().ToList();
            foreach (var member in activeMembers)
            {
                member.IsActive = true;
            }
            _dbcontext.SaveChanges();
        }

        public Member? GetMemberByCodeOnly(string memberCode)
        {
            memberCode = memberCode.Trim();
            return _dbcontext.Members.FirstOrDefault(m => m.Code.Contains(memberCode));
        }

        public Member GetMember(int memberID, bool? includeFamilyCount)
        {
            var member = _dbcontext.Members.Include(m => m.ClassMembers).First(m => m.MemberID == memberID);

            if (includeFamilyCount.HasValue && includeFamilyCount.Value)
            {
                var family = _dbcontext.Members.Where(m => m.UNFileNumber == member.UNFileNumber && m.MemberID != member.MemberID).OrderBy(m => m.Birthdate).ToList();
                member.FamilyMembers = family;
                member.FamilyCount = family.Count;
            }

            return member;
        }
        public List<Member> GetMembersCardData(List<int> memberIDs)
        {
            return _dbcontext.Members.Where(m => memberIDs.Contains(m.MemberID)).ToList();
        }

        public void UpdateMember(int memberID, string code,
            string? unFirstName,
            string? unLastName,
            string? baptismName,
            string? nickname,
            string unFileNumber,
            string unPersonalNumber,
            string? mobile,
            bool baptised,
            DateTime birthdate,
            char gender,
            string? school,
            string? work,
            string? notes,
            string? imageURL,
            string? s3ImageKey,
            string? imageReference,
            string? cardStatus,
            List<int>? classes,
            string modifiedByUserName)
        {
            Servant servant = GetServantByUsername(modifiedByUserName);
            Member originalMember = _dbcontext.Members.First(m => m.MemberID == memberID);
            Dictionary<string, string> auditTrail = new Dictionary<string, string>();

            bool isChanged = false;
            if (code != originalMember.Code)
            {
                auditTrail.Add("Code", string.Format("Old value: {0} new value:{1}", originalMember.Code, code));
                originalMember.Code = code;
                isChanged = true;
            }
            if (gender != originalMember.Gender)
            {
                auditTrail.Add("Gender", string.Format("Old value: {0} new value:{1}", originalMember.Gender, gender));
                originalMember.Gender = gender;
                isChanged = true;
            }
            if (unFirstName != originalMember.UNFirstName)
            {
                auditTrail.Add("UNFirstName", string.Format("Old value: {0} new value:{1}", originalMember.UNFirstName, unFirstName));
                originalMember.UNFirstName = unFirstName;
                isChanged = true;
            }
            if (unLastName != originalMember.UNLastName)
            {
                auditTrail.Add("UNLastName", string.Format("Old value: {0} new value:{1}", originalMember.UNLastName, unLastName));
                originalMember.UNLastName = unLastName;
                isChanged = true;
            }
            if (unFileNumber != originalMember.UNFileNumber)
            {
                auditTrail.Add("UNFileNumber", string.Format("Old value: {0} new value:{1}", originalMember.UNFileNumber, unFileNumber));
                originalMember.UNFileNumber = unFileNumber;
                isChanged = true;
            }
            if (unPersonalNumber != originalMember.UNPersonalNumber)
            {
                auditTrail.Add("UNPersonalNumber", string.Format("Old value: {0} new value:{1}", originalMember.UNPersonalNumber, unPersonalNumber));
                originalMember.UNPersonalNumber = unPersonalNumber;
                isChanged = true;
            }
            if (birthdate != originalMember.Birthdate)
            {
                auditTrail.Add("Birthdate", string.Format("Old value: {0} new value:{1}", originalMember.Birthdate.ToString("dd-MM-yyyy"), birthdate.ToString("dd-MM-yyyy")));
                originalMember.Birthdate = birthdate;
                isChanged = true;
            }
            if (baptised != originalMember.Baptised)
            {
                auditTrail.Add("Baptised", string.Format("Old value: {0} new value:{1}", originalMember.Baptised, baptised));
                originalMember.Baptised = baptised;
                isChanged = true;
            }
            if (cardStatus != originalMember.CardStatus)
            {
                auditTrail.Add("CardStatus", string.Format("Old value: {0} new value:{1}", originalMember.CardStatus, cardStatus));
                originalMember.CardStatus = cardStatus;
                isChanged = true;
            }
            if (imageReference != originalMember.ImageReference)
            {
                auditTrail.Add("ImageReference", string.Format("Old value: {0} new value:{1}", originalMember.ImageReference, imageReference));
                originalMember.ImageReference = imageReference;
                isChanged = true;
            }
            if (s3ImageKey != originalMember.S3ImageKey)
            {
                auditTrail.Add("S3ImageKey", string.Format("Old value: {0} new value:{1}", originalMember.S3ImageKey, s3ImageKey));
                originalMember.S3ImageKey = s3ImageKey;
                isChanged = true;
            }
            if (imageURL != originalMember.ImageURL)
            {
                auditTrail.Add("ImageURL", string.Format("Old value: {0} new value:{1}", originalMember.ImageURL, imageURL));
                originalMember.ImageURL = imageURL;
                isChanged = true;
            }
            if (mobile != originalMember.Mobile)
            {
                auditTrail.Add("Mobile", string.Format("Old value: {0} new value:{1}", originalMember.Mobile, mobile));
                originalMember.Mobile = mobile;
                isChanged = true;
            }
            if (nickname != originalMember.Nickname)
            {
                auditTrail.Add("Nickname", string.Format("Old value: {0} new value:{1}", originalMember.Nickname, nickname));
                originalMember.Nickname = nickname;
                isChanged = true;
            }
            if (school != originalMember.School)
            {
                auditTrail.Add("School", string.Format("Old value: {0} new value:{1}", originalMember.School, school));
                originalMember.School = school;
                isChanged = true;
            }
            if (work != originalMember.Work)
            {
                auditTrail.Add("Work", string.Format("Old value: {0} new value:{1}", originalMember.Work, work));
                originalMember.Work = work;
                isChanged = true;
            }
            if (notes != originalMember.Notes)
            {
                auditTrail.Add("Notes", string.Format("Old value: {0} new value:{1}", originalMember.Notes, notes));
                originalMember.Notes = notes;
                isChanged = true;
            }
            if (baptismName != originalMember.BaptismName)
            {
                auditTrail.Add("BaptismName", string.Format("Old value: {0} new value:{1}", originalMember.BaptismName, baptismName));
                originalMember.BaptismName = baptismName;
                isChanged = true;
            }
            var oldClasses = _dbcontext.ClassMembers.Where(cm => cm.MemberID == memberID).ToList();

            var oldIds = new List<int>(oldClasses.Select(m => m.ClassID));
            string deletedClasses = string.Empty;
            if (classes == null)
            {
                foreach (var cl in oldClasses)
                {
                    isChanged = true;
                    deletedClasses += cl.ClassID.ToString() + ",";
                    _dbcontext.ClassMembers.Remove(cl);

                }
            }
            else
            {
                var toBeDeleted = oldClasses.Where(m => !classes!.Contains(m.ClassID)).ToList();
                var toBeAdded = classes.Where(m => !oldIds.Contains(m)).ToList();

                string addedClasses = string.Empty;
                foreach (var cl in toBeAdded)
                {
                    isChanged = true;
                    _dbcontext.ClassMembers.Add(new ClassMember() { MemberID = memberID, ClassID = cl });
                    addedClasses += cl.ToString() + ",";
                }
                if (string.IsNullOrEmpty(addedClasses))
                    auditTrail.Add("Added Classes", addedClasses.TrimEnd(','));

                foreach (var cl in toBeDeleted!)
                {
                    isChanged = true;
                    deletedClasses += cl.ClassID.ToString() + ",";
                    _dbcontext.ClassMembers.Remove(cl);

                }
            }
            if (string.IsNullOrEmpty(deletedClasses))
                auditTrail.Add("Deleted Classes", deletedClasses.TrimEnd(','));

            if (isChanged)
            {
                originalMember.ModifiedBy = servant.ServantID;
                originalMember.ModifiedAt = CurrentTime;
                originalMember.ModifiedLog = JsonSerializer.Serialize(auditTrail);

                AuditTrail trail = new AuditTrail()
                {
                    AiditTrail = JsonSerializer.Serialize(auditTrail),
                    EntityID = code,
                    EntityName = "Member",
                    ServantName = servant.ServantName,
                    Timestamp = CurrentTime
                };
                _dbcontext.AuditTrail.Add(trail);
            }
            _dbcontext.SaveChanges();
        }
        public void UpdateMember(Member member, string modifiedByUserName)
        {
            Servant servant = GetServantByUsername(modifiedByUserName);
            Member originalMember = _dbcontext.Members.First(m => m.MemberID == member.MemberID);
            Dictionary<string, string> auditTrail = new Dictionary<string, string>();
            if (originalMember == null)
            {
                throw new Exception("Member not found");
            }
            bool isChanged = false;
            if (member.Gender != originalMember.Gender)
            {
                auditTrail.Add("Gender", string.Format("Old value: {0} new value:{1}", originalMember.Gender, member.Gender));
                originalMember.Gender = member.Gender;
                isChanged = true;
            }
            if (member.UNFirstName != originalMember.UNFirstName)
            {
                auditTrail.Add("UNFirstName", string.Format("Old value: {0} new value:{1}", originalMember.UNFirstName, member.UNFirstName));
                originalMember.UNFirstName = member.UNFirstName;
                isChanged = true;
            }
            if (member.UNLastName != originalMember.UNLastName)
            {
                auditTrail.Add("UNLastName", string.Format("Old value: {0} new value:{1}", originalMember.UNLastName, member.UNLastName));
                originalMember.UNLastName = member.UNLastName;
                isChanged = true;
            }
            if (member.UNFileNumber != originalMember.UNFileNumber)
            {
                auditTrail.Add("UNFileNumber", string.Format("Old value: {0} new value:{1}", originalMember.UNFileNumber, member.UNFileNumber));
                originalMember.UNFileNumber = member.UNFileNumber;
                isChanged = true;
            }
            if (member.UNPersonalNumber != originalMember.UNPersonalNumber)
            {
                auditTrail.Add("UNPersonalNumber", string.Format("Old value: {0} new value:{1}", originalMember.UNPersonalNumber, member.UNPersonalNumber));
                originalMember.UNPersonalNumber = member.UNPersonalNumber;
                isChanged = true;
            }
            if (member.Birthdate != originalMember.Birthdate)
            {
                auditTrail.Add("Birthdate", string.Format("Old value: {0} new value:{1}", originalMember.Birthdate.ToString("dd-MM-yyyy"), member.Birthdate.ToString("dd-MM-yyyy")));
                originalMember.Birthdate = member.Birthdate;
                isChanged = true;
            }
            if (member.Baptised != originalMember.Baptised)
            {
                auditTrail.Add("Baptised", string.Format("Old value: {0} new value:{1}", originalMember.Baptised, member.Baptised));
                originalMember.Baptised = member.Baptised;
                isChanged = true;
            }
            if (member.CardStatus != originalMember.CardStatus)
            {
                auditTrail.Add("CardStatus", string.Format("Old value: {0} new value:{1}", originalMember.CardStatus, member.CardStatus));
                originalMember.CardStatus = member.CardStatus;
                isChanged = true;
            }
            if (member.ImageReference != originalMember.ImageReference)
            {
                auditTrail.Add("ImageReference", string.Format("Old value: {0} new value:{1}", originalMember.ImageReference, member.ImageReference));
                originalMember.ImageReference = member.ImageReference;
                isChanged = true;
            }
            if (member.Mobile != originalMember.Mobile)
            {
                auditTrail.Add("Mobile", string.Format("Old value: {0} new value:{1}", originalMember.Mobile, member.Mobile));
                originalMember.Mobile = member.Mobile;
                isChanged = true;
            }
            if (member.Nickname != originalMember.Nickname)
            {
                auditTrail.Add("Nickname", string.Format("Old value: {0} new value:{1}", originalMember.Nickname, member.Nickname));
                originalMember.Nickname = member.Nickname;
                isChanged = true;
            }
            if (member.School != originalMember.School)
            {
                auditTrail.Add("School", string.Format("Old value: {0} new value:{1}", originalMember.School, member.School));
                originalMember.School = member.School;
                isChanged = true;
            }
            if (member.Work != originalMember.Work)
            {
                auditTrail.Add("Work", string.Format("Old value: {0} new value:{1}", originalMember.Work, member.Work));
                originalMember.Work = member.Work;
                isChanged = true;
            }
            if (member.Notes != originalMember.Notes)
            {
                auditTrail.Add("Notes", string.Format("Old value: {0} new value:{1}", originalMember.Notes, member.Notes));
                originalMember.Notes = member.Notes;
                isChanged = true;
            }
            if (isChanged)
            {
                originalMember.ModifiedBy = servant.ServantID;
                originalMember.ModifiedAt = CurrentTime;
                originalMember.ModifiedLog = JsonSerializer.Serialize(auditTrail);
                _dbcontext.SaveChanges();
            }
        }

        public Member CreateMember(Member member, string modifiedByUserName)
        {
            Servant servant = GetServantByUsername(modifiedByUserName);
            int sequence = 0;
            Member newMember = new Member();
            newMember.Gender = member.Gender;
            newMember.UNFirstName = member.UNFirstName;
            newMember.UNLastName = member.UNLastName;
            newMember.UNFileNumber = member.UNFileNumber;
            newMember.UNPersonalNumber = member.UNPersonalNumber;
            newMember.Birthdate = member.Birthdate;
            newMember.Baptised = member.Baptised;
            newMember.ImageReference = member.ImageReference;
            newMember.Mobile = member.Mobile;
            newMember.Nickname = member.Nickname;
            newMember.School = member.School;
            newMember.Work = member.Work;
            newMember.Notes = member.Notes;
            newMember.CreatedBy = servant.ServantID;
            newMember.CreatedAt = CurrentTime;
            newMember.Code = GenerateCode(member.Gender, member.Birthdate, out sequence);
            newMember.Sequence = sequence;
            newMember.S3ImageKey = member.S3ImageKey;
            newMember.ImageURL = member.ImageURL;
            if ((CurrentTime.Year - newMember.Birthdate.Year) <= 5)
                newMember.CardStatus = "NotApplicable";
            else
                newMember.CardStatus = string.IsNullOrEmpty(newMember.ImageURL) ? "MissingPhoto" : "ReadyToPrint";
            newMember.IsActive = true;
            newMember.IsMainMember = newMember.Age >= 20;
            newMember.ModifiedLog = "Member created.";

            _dbcontext.Members.Add(newMember);
            _dbcontext.SaveChanges();
            return newMember;
        }

        public Member CreateMember(string? unFirstName,
            string? unLastName,
            string? baptismName,
            string? nickname,
            string unFileNumber,
            string unPersonalNumber,
            string? mobile,
            bool baptised,
            DateTime birthdate,
            char gender,
            string? school,
            string? work,
            string? notes,
            string? imageURL,
            string? s3ImageKey,
            string? imageReference,
            List<int>? classes,
            string modifiedByUserName)
        {
            if (_dbcontext.Members.Any(m => m.UNPersonalNumber == unPersonalNumber))
            {
                var ex = new Exception("UN Personal number already exists!");
                ex.Source = "Show message";
                throw ex;
            }
            Servant servant = GetServantByUsername(modifiedByUserName);
            int sequence = 0;
            Member newMember = new Member();
            newMember.Gender = gender;
            newMember.UNFirstName = unFirstName;
            newMember.UNLastName = unLastName;
            newMember.UNFileNumber = unFileNumber;
            newMember.UNPersonalNumber = unPersonalNumber;
            newMember.Birthdate = birthdate;
            newMember.Baptised = baptised;
            newMember.BaptismName = baptismName;
            newMember.ImageReference = imageReference;
            newMember.Mobile = mobile;
            newMember.Nickname = nickname;
            newMember.School = school;
            newMember.Work = work;
            newMember.Notes = notes;
            newMember.CreatedBy = servant.ServantID;
            newMember.CreatedAt = CurrentTime;
            newMember.Code = GenerateCode(gender, birthdate, out sequence);
            newMember.Sequence = sequence;
            newMember.S3ImageKey = s3ImageKey;
            newMember.ImageURL = imageURL;
            if ((CurrentTime.Year - newMember.Birthdate.Year) <= 5)
                newMember.CardStatus = "NotApplicable";
            else
                newMember.CardStatus = string.IsNullOrEmpty(newMember.ImageURL) ? "MissingPhoto" : "ReadyToPrint";
            newMember.IsActive = false;
            newMember.IsDeleted = false;
            newMember.IsMainMember = newMember.Age >= 20;
            newMember.ModifiedLog = "Member created.";

            _dbcontext.Members.Add(newMember);
            _dbcontext.SaveChanges();

            if (classes != null)
            {
                foreach (var cl in classes)
                {
                    _dbcontext.ClassMembers.Add(new ClassMember() { MemberID = newMember.MemberID, ClassID = cl });
                }
                _dbcontext.SaveChanges();
            }
            return newMember;
        }


        public List<Member> GetMembersByCardStatus(CardStatus status)
        {
            return _dbcontext.Members.Where(m => m.CardStatus.ToLower() == status.ToString().ToLower()).OrderByDescending(m => m.ModifiedAt).ToList();
        }
        public List<Member> GetMembers(string memberCode, string firstName, string lastName)
        {
            List<Member> members = new List<Member>();
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                memberCode = memberCode.Trim();
                var member = _dbcontext.Members.
                    FirstOrDefault(m => m.Code.Contains(memberCode) ||
                    m.UNPersonalNumber == memberCode || m.UNFileNumber == memberCode
                    || m.ImageReference == memberCode);

                if (member == null)
                    return members;
                if (string.IsNullOrEmpty(member.UNFileNumber))
                {
                    members.Add(member);
                    return members;
                }
                members = _dbcontext.Members.Where(m => m.UNFileNumber == member.UNFileNumber).OrderBy(m => m.Birthdate).ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    members = _dbcontext.Members.Where(m => m.UNLastName.Contains(lastName)).ToList();
                }
                else if (!string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                {
                    members = _dbcontext.Members.Where(m => m.UNFirstName.Contains(firstName)).ToList();
                }
                else
                {
                    members = _dbcontext.Members.Where(m => m.UNFirstName.Contains(firstName) && m.UNLastName.Contains(lastName)).ToList();
                }
            }
            return members;

        }

        public string GenerateCode(char gender, DateTime birthdate, out int sequence)
        {
            int seq = _dbcontext.Members.Select(m => m.Sequence).Max() + 1;
            sequence = seq;
            return string.Format("{0}{1}-{2}", birthdate.ToString("yy"), gender, seq.ToString("0000"));
        }
        public bool ValidateUNNumber(string unPersonalNo, int? memberID)
        {
            return !_dbcontext.Members.Any(m => m.UNPersonalNumber.ToLower() == unPersonalNo.ToLower() && m.MemberID != memberID);
        }

        public bool UpdateCardStatus(string memberCode, CardStatus cardStatus)

        {
            var member = GetMemberByCodeOnly(memberCode);
            if (member == null)
            {
                return false;
            }
            if (member.CardStatus != "Delivered" && cardStatus == CardStatus.Delivered)
            {
                member.CardDeliveryCount++;
            }
            member.CardStatus = cardStatus.ToString();
            member.ModifiedAt = CurrentTime;
            _dbcontext.SaveChanges();
            return true;
        }
        // Get member event registrations
        public List<EventRegistration> GetMemberEventRegistrations(int memberId)
        {
            return _dbcontext.EventRegistrations
                .Include(e => e.Event)
                .Where(e => e.MemberID == memberId)
                .OrderByDescending(e => e.Event.EventStartDate)
                .ToList();
        }

        // Get member classes
        public List<ClassMember> GetMemberClasses(int memberId)
        {
            return _dbcontext.ClassMembers
                .Include(c => c.Class)
                    .ThenInclude(c => c.Meeting)
                .Include(c => c.Servant)
                .Where(c => c.MemberID == memberId)
                .ToList();
        }

        public List<MemberClassOverview> GetMemberClassOverviews(int memberID)
        {
            var memberClasses = _dbcontext.ClassMembers.Include(cm => cm.Class).Where(mc => mc.MemberID == memberID).ToList();
            if (memberClasses == null)
                return new List<MemberClassOverview>();

            var memberAttendances = _dbcontext.ClassAttendances.Include(c => c.ClassOccurrence).Where(ca => ca.MemberID == memberID).ToList();
            List<MemberClassOverview> membersExtended = new List<MemberClassOverview>();
            foreach (var mc in memberClasses)
            {
                MemberClassOverview mco = new MemberClassOverview();
                mco.ClassName = mc.Class.ClassName;
                int occurrencesCount = _dbcontext.ClassOccurrences.Count(c => c.ClassID == mc.ClassID && c.ClassOccurrenceStartDate <= CurrentTime);
                var memberAttendance = memberAttendances.Where(c => c.ClassOccurrence.ClassID == mc.ClassID).ToList();
                if (memberAttendance != null && memberAttendance.Count > 0)
                {
                    mco.LastPresentDate = memberAttendance.Max(c => c.ClassOccurrence.ClassOccurrenceStartDate);
                    mco.Attendance = string.Format("{0}/{1}", memberAttendance.Count.ToString("00"), occurrencesCount);
                }
                membersExtended.Add(mco);
            }

            return membersExtended;
        }
        public void UpdateMemberImage(int memberID, string imageURL, string key)
        {
            var member = _dbcontext.Members.Where(m => m.MemberID == memberID).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("Member not found");
            }
            member.ImageURL = imageURL;
            member.S3ImageKey = key;
            _dbcontext.SaveChanges();
        }
        public List<string> BulkUploadImages(List<IamgeProperties> membersImages)
        {
            List<string> missingMembers = new List<string>();
            var imageFilenames = membersImages.Select(r => r.Filename).ToList();

            var members = _dbcontext.Members.Where(m => imageFilenames.Contains(m.ImageReference)).ToList();

            foreach (var item in membersImages)
            {
                var member = members.Where(m => m.ImageReference == item.Filename).FirstOrDefault();
                if (member != null)
                {
                    member.ImageURL = item.ImageURL;
                    member.S3ImageKey = item.Key;
                }
                else
                {
                    missingMembers.Add(item.Key);
                }
            }
            _dbcontext.SaveChanges();
            return missingMembers;
        }

        // Get member attendance history
        public List<ClassAttendance> GetAttendanceHistory(int memberId)
        {
            return _dbcontext.ClassAttendances
                .Include(a => a.ClassOccurrence)
                    .ThenInclude(co => co.Class)
                .Include(a => a.Servant)
                .Where(a => a.MemberID == memberId)
                .OrderByDescending(a => a.ClassOccurrence.ClassOccurrenceStartDate)
                .ToList();
        }

        // Get member aid participations
        public List<MemberAid> GetMemberAids(int memberId)
        {
            return _dbcontext.MemberAids
                .Include(a => a.Aid)
                .Include(a => a.Servant)
                .Where(a => a.MemberID == memberId)
                .OrderByDescending(a => a.TimeStamp)
                .ToList();
        }

        // Get member fund transactions
        public List<MemberFund> GetMemberFunds(int memberId)
        {
            return _dbcontext.MemberFunds
                .Include(f => f.Servant)
                .Where(f => f.MemberID == memberId)
                .OrderByDescending(f => f.RequestDate)
                .ToList();
        }

        public List<object> GetAllCodes()
        {
            return _dbcontext.Members.OrderBy(m => m.Sequence).Select(m => new { m.MemberID, m.Code }).Cast<object>().ToList();
        }

        public void UpdateMemberMobile(int memberID, string mobile, string? modifiedByUserName)
        {
            Servant servant = GetServantByUsername(modifiedByUserName);
            Member originalMember = _dbcontext.Members.First(m => m.MemberID == memberID);
            Dictionary<string, string> auditTrail = new Dictionary<string, string>();
            if (originalMember == null)
            {
                throw new Exception("Member not found");
            }
            bool isChanged = false;
            if (mobile != originalMember.Mobile)
            {
                auditTrail.Add("Mobile", string.Format("Old value: {0} new value:{1}", originalMember.Mobile, mobile));
                originalMember.Mobile = mobile;
                isChanged = true;
            }

            if (isChanged)
            {
                originalMember.ModifiedBy = servant.ServantID;
                originalMember.ModifiedAt = CurrentTime;
                originalMember.ModifiedLog = JsonSerializer.Serialize(auditTrail);

                AuditTrail trail = new AuditTrail()
                {
                    AiditTrail = JsonSerializer.Serialize(auditTrail),
                    EntityID = originalMember.Code,
                    EntityName = "Member",
                    ServantName = servant.ServantName,
                    Timestamp = CurrentTime
                };
                _dbcontext.AuditTrail.Add(trail);
                _dbcontext.SaveChanges();
            }
        }

        public class IamgeProperties
        {
            public string Filename { get; set; }
            public string Key { get; set; }
            public string ImageURL { get; set; }
        }
    }
}
