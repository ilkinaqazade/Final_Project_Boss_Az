# Job Search Application

This project simulates a job search application that acts as a bridge between job seekers, workers, and job offices that create job listings. Workers can get information about job offices and job postings, apply for jobs, and job offices can review CVs.

## Technologies Used

This project is written in C#. Additionally, the Newtonsoft.Json library is used to store user data and job listings.

## Project Structure

- The `JobSearching` class contains the core logic of the project and coordinates operations.
- The `UserData` and `JobData` classes represent user and job listing data.
- The `JsonDataManager` class manages loading and saving data to JSON files.
- The `Worker`, `JobOffice`, and `JobSearching` classes define worker, job office, and job listing objects.

## User Guide

1. **Registration**

   - To register as a worker, use the "Register Worker" option and enter a valid email address and password.
   - To register as a job office, use the "Register Office" option and enter a valid email address and password.

2. **Logging In**

   - After registration, log in using the "Login Worker" or "Login Office" options.

3. **Creating Job Listings**

   - If you are logged in as a job office, you can create new job listings using the "Create Job Posting" option.

4. **Creating a CV**

   - If you are logged in as a worker, you can create your own CV using the "Create CV" option.

5. **Applying for Jobs**

   - If you are logged in as a worker, you can apply for job listings using the "Apply to Job Posting" option.

6. **CV Review (For Job Offices Only)**

   - If you are logged in as a job office, you can review CVs of workers who have applied for jobs using the "Wiew CV" option.


