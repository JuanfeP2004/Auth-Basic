# Basic Authentication Proyect

This project is an experiment to test the creation of a Schema of Authentication/Authorization based in roles, the project was only made with recreative/demostrative purposes.

## Technologies

The project was made with the .NET framework in mind, for this many of this stack was meant to have good integration with this aprroach, the technologies used are the following:

* C# 10 (ASP.NET): the backend of the service
* Microsoft SQL Server 2025: database (put in a docker container)
* React.js: a popular front end framework

(Note that the main emphasis will be put in the backend part, while the others will be more basic/straightfoward)


## Description
The project has the following functionalities:

* Registry an user with a email, name and password
* Allows the user login with email and password, along with 2FA
* Allows the user with a code reset his password
* Limit access to the app based in user roles
* Grant the admin user give or remove individual users roles

Also, the roles that the app have are the following:

* GiveRole: allows grant roles, only the admin has this role
* RemoveRole: allows remove roles, only the admin has this role
* ReadLogs: allows read the logs
* LockUser: allows disable an user
* UnlockUser: allows active an user
* Role 1-3: generics roles with no use in this project

The proyect also have a registry of logs to track the system, the following are the messages of the log system:

* User #### was registered
* User #### was logged
* User #### was unlogged
* User #### was locked
* User #### was unlocked
* User #### request recovery code
* User #### was granted ####
* User #### was removed ####

## Architecture
The following is the architecture of the database:
(https://drive.google.com/file/d/192o3dTGmEz4rTVbWOoa8zVwML1VztGdV/view?usp=sharing)

## Instalation & Deployment
(In progress)