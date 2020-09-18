using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class TransfereePortalDto
    {
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string System { get; set; }
        public string Avatar
        {
            get
            {
                string avatar = null;

                if(!string.IsNullOrEmpty(FirstName))
                {
                    avatar = FirstName.ToUpper().Substring(0, 1);

                    if (!string.IsNullOrEmpty(LastName))
                    {
                        avatar += LastName.ToUpper().Substring(0, 1);
                    }
                    else
                    {
                        if (FirstName.Length > 2)
                        {
                            avatar += FirstName.ToUpper().Substring(1, 1);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(LastName))
                    {
                        if (LastName.Length > 1)
                        {
                            avatar = LastName.ToUpper().Substring(0, 2);
                        }
                        else
                        {
                            avatar = LastName.ToUpper().Substring(0, 1);
                        }
                    }
                }

                return avatar;
            }
        }
        public IList<ServiceDto> Services { get; set; }
    }
}
