using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;

#region Boos_Project_Az


namespace Boss_Project_Az
{
    public class JobSearching
    {
        private UserData userData;
        private JobData jobData;

        public JobSearching()
        {
            userData = JsonDataManager.LoadUserData();
            jobData = JsonDataManager.LoadJobData();
        }


        public void ConsoleDesign()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(@"▀█████████▄   ▄██████▄   ▄██████▄     ▄████████ 
  ███    ███ ███    ███ ███    ███   ███    ███ 
  ███    ███ ███    ███ ███    ███   ███    █▀  
 ▄███▄▄▄██▀  ███    ███ ███    ███   ███        
▀▀███▀▀▀██▄  ███    ███ ███    ███ ▀███████████ 
  ███    ██▄ ███    ███ ███    ███          ███ 
  ███    ███ ███    ███ ███    ███    ▄█    ███ 
▄█████████▀   ▀██████▀   ▀██████▀   ▄████████▀  
                                                ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public bool IsEmailValid(string email)
        {
            string[] validEmailDomains = { "gmail.com", "yahoo.com", "mail.ru", "outlook.com" };

            string emailDomain = email.Split('@').Last().ToLower();

            return validEmailDomains.Contains(emailDomain);
        }


        public void RegisterJobWorker()
        {
            Console.Clear();
            Console.WriteLine("********************************");
            Console.WriteLine("           Worker Register");
            string mail;
            do
            {
                Console.Write("Enter mail: ");
                mail = Console.ReadLine();
                if (!IsEmailValid(mail))
                {
                    Console.WriteLine("Invalid email address. Please use a valid email address.");
                }
            } while (!IsEmailValid(mail));

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var newWorker = new Worker
            {
                Id = userData.Workers.Count + 1,
                WorkerMail = mail,
                WorkerPassword = password
            };

            userData.Workers.Add(newWorker);
            JsonDataManager.SaveUserData(userData);
        }




        public void RegisterJobOffice()
        {
            Console.Clear();
            Console.WriteLine("********************************");
            Console.WriteLine("           Office Register");
            string mail;
            do
            {
                Console.Write("Enter mail: ");
                mail = Console.ReadLine();
                if (!IsEmailValid(mail))
                {
                    Console.WriteLine("Invalid email address. Please use a valid email address.");
                }
            } while (!IsEmailValid(mail));

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var newOffice = new JobOffice
            {
                Id = userData.Offices.Count + 1,
                OfficeMail = mail,
                OfficePassword = password
            };

            userData.Offices.Add(newOffice);
            JsonDataManager.SaveUserData(userData);
        }


        public void CreateJobPosting(JobOffice office)
        {
            Console.Clear();
            Console.WriteLine("*************************");
            Console.WriteLine("   Create Job Posting\n");
            Console.Write("Enter Job Name: ");
            string jobName = Console.ReadLine();
            Console.Write("Enter Job Info: ");
            string jobInfo = Console.ReadLine();
            Console.Write("Enter Job Salary: ");
            double jobSalary = double.Parse(Console.ReadLine());

            int jobId = jobData.JobPostings.Count > 0 ? jobData.JobPostings.Max(j => j.JobId) + 1 : 1;

            var newJob = new JobSearcing
            {
                JobId = jobId,
                JobName = jobName,
                JobInfo = jobInfo,
                JobSalary = jobSalary,
                CreatedByOfficeId = office.Id
            };

            jobData.JobPostings.Add(newJob);
            JsonDataManager.SaveJobData(jobData);
            Console.WriteLine("Job posting created successfully!");
        }

        public void JobList()
        {
            Console.Clear();
            ConsoleDesign();
            foreach (var item in jobData.JobPostings)
            {
                Console.WriteLine($"Job Id: {item.JobId}");
                Console.WriteLine($"Job Name: {item.JobName}");
                Console.WriteLine($"Job Info: {item.JobInfo}");
                Console.WriteLine($"Job Salary: {item.JobSalary}");
                Console.WriteLine();
            }

            ReturnToMainMenu(); 
        }



        public void CreateWorkerCV(Worker worker)
        {
            Console.Clear();
            Console.WriteLine("*************************");
            Console.WriteLine("   Create Worker CV\n");

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Surname: ");
            string surname = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            string phoneNumber = Console.ReadLine();
            Console.Write("Enter Address: ");
            string address = Console.ReadLine();
            Console.Write("Enter Skills (comma separated): ");
            string skillsInput = Console.ReadLine();

            List<string> skills = skillsInput.Split(',').Select(skill => skill.Trim()).ToList();


            worker.WorkerName = name;
            worker.WorkerSurname = surname;
            worker.WorkerPhoneNumber = phoneNumber;
            worker.WorkerAddress = address;
            worker.WorkerSkill = skillsInput;

            JsonDataManager.SaveUserData(userData);

            Console.WriteLine("CV created successfully!");
        }

        public void ApplyToJobPosting(Worker worker)
        {
            if (string.IsNullOrEmpty(worker.WorkerSkill))
            {
                Console.WriteLine("You need to create a CV before applying to job postings.");
                Console.WriteLine("Do you want to create a CV now? (Y/N)");
                string response = Console.ReadLine();

                if (response.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    CreateWorkerCV(worker);
                }
                else
                {
                    Console.WriteLine("CV creation is required to apply for job postings.");
                    return;
                }
            }
            Console.Clear();
            Console.WriteLine("Available Job Postings:");
            JobList();
            Console.Write("Enter the Job Id you want to apply for: ");
            int jobId = int.Parse(Console.ReadLine());

            var jobPosting = jobData.JobPostings.FirstOrDefault(job => job.JobId == jobId);

            if (jobPosting != null)
            {
                Console.WriteLine($"You have successfully applied for the job: {jobPosting.JobName}");

                var office = userData.Offices.FirstOrDefault(o => o.Id == jobPosting.CreatedByOfficeId);
                if (office != null)
                {
                    office.StoreCV(worker.WorkerName, worker.WorkerSurname, worker.WorkerSkill);
                }
            }
            else
            {
                Console.WriteLine("Invalid Job Id. Application failed.");
            }

            ReturnToMainMenu();
        }


        public void ViewCVs(JobOffice office)
        {
            Console.Clear();
            Console.WriteLine("*************************");
            Console.WriteLine("   View Received CVs\n");

            foreach (var cv in office.CVList)
            {
                Console.WriteLine($"Worker Id: {cv.WorkerId}");
                Console.WriteLine($"Worker Name: {cv.WorkerName}");
                Console.WriteLine($"Worker Surname: {cv.WorkerSurname}");
                Console.WriteLine($"Worker Skills: {cv.WorkerSkills}");
                Console.WriteLine();
            }

            Console.WriteLine("************************");
            Console.Write("Select Accept Id : ");
            int acceptCv = int.Parse(Console.ReadLine());

            var acceptedWorker = office.CVList.FirstOrDefault(cv => cv.WorkerId == acceptCv);
            if (acceptedWorker != null)
            {
                try
                {
                    string senderEmail = "gmail.com";
                    string senderPassword = "password";

                    string recipientEmail = acceptedWorker.WorkerName;

                    SmtpClient client = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(senderEmail, senderPassword),
                        EnableSsl = true,
                    };

                    MailMessage mail = new MailMessage(senderEmail, recipientEmail)
                    {
                        Subject = "Accept Job",
                        Body = "Your application has been accepted, congratulations !"
                    };

                    client.Send(mail);

                    Console.WriteLine("E-posta başarıyla gönderildi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
                }
            }
            else
            {
                JobMenu();
            }
        }


        private void ReturnToMainMenu()
        {
            Console.WriteLine("Press Enter to return to the main menu...");
            Console.ReadLine();
        }

        public void LoginJobWorker()
        {
            Console.Clear();
            ConsoleDesign();
            Console.WriteLine("********************************");
            Console.WriteLine("        Worker Login");
            Console.Write("Enter mail: ");
            string mail = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var worker = userData.Workers.SingleOrDefault(w => w.WorkerMail == mail && w.WorkerPassword == password);

            if (worker != null)
            {
                Console.Clear();
                ConsoleDesign();
                Console.WriteLine("************* Welcome Worker *************\n");
                Console.WriteLine($"Worker Logged In: {worker.WorkerName}");
                Console.WriteLine("[1] Create CV");
                Console.WriteLine("[2] List Applications");
                Console.WriteLine("[3] Apply to Job Posting");
                Console.WriteLine("[4] Back To Login");
                Console.Write("Enter: ");
                int select = int.Parse(Console.ReadLine());

                switch (select)
                {
                    case 1:
                        CreateWorkerCV(worker);
                        break;
                    case 2:
                        JobList();
                        break;
                    case 3:
                        ApplyToJobPosting(worker);
                        break;
                    case 4:
                        JobMenu();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine("Login Failed. Invalid email or password.");
            }
        }

        public void LoginJobOffice()
        {
            Console.Clear();
            ConsoleDesign();
            Console.WriteLine("********************************");
            Console.WriteLine("          Office Login");
            Console.Write("Enter mail: ");
            string mail = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var office = userData.Offices.SingleOrDefault(o => o.OfficeMail == mail && o.OfficePassword == password);

            if (office != null)
            {
                Console.Clear();
                ConsoleDesign();
                Console.WriteLine("[1] Create Job Posting");
                Console.WriteLine("[2] List Applications");
                Console.WriteLine("[3] Wiew CV");
                Console.WriteLine("[4] Back To Login");
                Console.Write("Enter: ");
                int select = int.Parse(Console.ReadLine());

                switch (select)
                {
                    case 1:
                        CreateJobPosting(office);
                        break;
                    case 2:
                        JobList();
                        break;
                    case 3:
                        ViewCVs(office);
                        break;
                    case 4:
                        JobMenu();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine("Login Failed. Invalid email or password.");
            }
        }

        private int selectedMenuItem = 1;

        public void JobMenu()
        {
            string[] menuItems = { "Register Worker", "Register Office", "Login Worker", "Login Office" };
            int totalMenuItems = menuItems.Length;

            while (true)
            {


                Console.Clear();
                ConsoleDesign();
                Console.WriteLine();

                for (int i = 0; i < totalMenuItems; i++)
                {
                    if (i == selectedMenuItem - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.WriteLine($"[{i + 1}] {menuItems[i]}");
                    Console.ForegroundColor = ConsoleColor.White;

                }

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    selectedMenuItem = selectedMenuItem > 1 ? selectedMenuItem - 1 : totalMenuItems;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedMenuItem = selectedMenuItem < totalMenuItems ? selectedMenuItem + 1 : 1;
                }
                else if (key == ConsoleKey.Enter)
                {
                    HandleMenuSelection(selectedMenuItem);
                }
            }
        }

        private void HandleMenuSelection(int selectedMenuItem)
        {
            switch (selectedMenuItem)
            {
                case 1:
                    RegisterJobWorker();
                    break;
                case 2:
                    RegisterJobOffice();
                    break;
                case 3:
                    LoginJobWorker();
                    break;
                case 4:
                    LoginJobOffice();
                    break;
                default:
                    break;
            }
        }
    }

    public class Worker
    {
        private static int lastWorkerId = 0;

        public int Id { get; set; }
        public string? WorkerName { get; set; }
        public string? WorkerSurname { get; set; }
        public string? WorkerMail { get; set; }
        public string? WorkerPassword { get; set; }
        public string? WorkerPhoneNumber { get; set; }
        public string? WorkerAddress { get; set; }
        public string? WorkerSkill { get; set; }

        public Worker()
        {
            lastWorkerId++;
            Id = lastWorkerId;
        }
    }


    public class WorkerCV
    {
        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public string WorkerSurname { get; set; }
        public string WorkerSkills { get; set; }
    }

    public class JobOffice
    {
        public int Id { get; set; }
        public string? OfficeName { get; set; }
        public string? OfficePhoneNumber { get; set; }
        public string? OfficeMail { get; set; }
        public string? OfficePassword { get; set; }
        public string? OfficeAddress { get; set; }

        public List<WorkerCV> CVList { get; set; } = new List<WorkerCV>();

        public void StoreCV(string workerName, string workerSurname, string workerSkills)
        {
            var cv = new WorkerCV
            {
                WorkerName = workerName,
                WorkerSurname = workerSurname,
                WorkerSkills = workerSkills
            };

            CVList.Add(cv);
        }
    }

    public class JobSearcing
    {
        public int JobId { get; set; }
        public string? JobName { get; set; }
        public string? JobInfo { get; set; }
        public double JobSalary { get; set; }

        public int CreatedByOfficeId { get; set; }
    }

    public class UserData
    {
        public List<Worker> Workers { get; set; } = new List<Worker>();
        public List<JobOffice> Offices { get; set; } = new List<JobOffice>();
    }

    public class JobData
    {
        public List<JobSearcing> JobPostings { get; set; } = new List<JobSearcing>();
    }

    public static class JsonDataManager
    {
        private static readonly string UserDataFilePath = "UserData.json";
        private static readonly string JobDataFilePath = "JobData.json";

        public static UserData LoadUserData()
        {
            if (File.Exists(UserDataFilePath))
            {
                string json = File.ReadAllText(UserDataFilePath);
                return JsonConvert.DeserializeObject<UserData>(json);
            }
            return new UserData();
        }

        public static void SaveUserData(UserData userData)
        {
            string json = JsonConvert.SerializeObject(userData);
            File.WriteAllText(UserDataFilePath, json);
        }

        public static JobData LoadJobData()
        {
            if (File.Exists(JobDataFilePath))
            {
                string json = File.ReadAllText(JobDataFilePath);
                return JsonConvert.DeserializeObject<JobData>(json);
            }
            return new JobData();
        }

        public static void SaveJobData(JobData jobData)
        {
            string json = JsonConvert.SerializeObject(jobData);
            File.WriteAllText(JobDataFilePath, json);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            JobSearching jobSearching = new JobSearching();

            jobSearching.JobMenu();
        }
    }
}

#endregion