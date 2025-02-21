!!! Актуальная версия проекта лежит в ветке develop !!!

!!! К сожалению, я на начальных этапах разработки забыл добавить gitignore)))))) !!!

The Medical Information System API provides a backend service for managing medical records, doctors, patients, and scheduled visits. The system follows a RESTful architecture and supports authentication, profile management, inspections, and email notifications.

Technologies Used

	.NET (ASP.NET Core) – Backend framework
	
	EFC (Entity Framework) - Backend framework
	
	MySQL – Database for storing medical records
	
	JWT – Authentication mechanism
	
	Quartz.NET – Task scheduling for automated jobs
	
	MailDev/SMTP – Email notifications

Features

	Doctor Authentication & Authorization
	
		Register, login, and logout
		
		JWT-based authentication
		
		Token blacklist for logout security

	Patient Management
	
		Create, update, and retrieve patient records
	
	Medical Inspections
	
		Assign diagnoses to inspections
		
		Delete diagnoses along with inspections
		
		Retrieve patient inspection history
	
	Doctor Profile Management
	
		View and edit personal profiles
		
	Scheduled Tasks
	
		Automated cleanup of expired tokens

		Email notifications for missed scheduled visits
