# Devbobs's Notebook 
**Team:** Devbobs  
**Contributors:** Marius, Morten, Jonas, Torkil & Laura

---

#### Format
Header with Lecture number.

---

#### What should be written down
- Keep a record of **when** you did **what**. 
- Note down **what went wrong**, **where** you found a **solution**, and **keep links** for that.


# Lecture 02
03/02 | 10:01: Created notebook.md <br>
03/02 | 10:10: Created project board for issues.<br>
- Issues with board not being public - fixed.

03/02 | 11:40: Imported and refactored Chirp to replace minitwit
- Copied chirp project files
- Refactored to remove OAuth
- Can be run with `dotnet run`.
- Confused about how we should handle tests. Asked TA - we need to choose what we want. Decided to cut and paste in current minitwit_tests.py file to make it work with Chirp.

03/02 | 22:50: Created dockerfile to containerize Chirp / Minitwit. 
- created _dockerfile_ branch.
- Looked at guides: https://medium.com/@aliyildizoz/understanding-asp-net-core-dockerfile-a523233bb9a4 & https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux&pivots=dotnet-8-0 & https://hub.docker.com/r/microsoft/dotnet-runtime
- Had problems copying files, looked at https://stackoverflow.com/questions/74120448/understanding-this-docker-file-where-are-the-files-being-copied
- create docker image with `docker build -t marho/mychirp .` This had an issue with running forever.

07/02 | 13:00: Fixed issue with docker file running forever
- Found it was because the dockerfile was running dotnet run, which ofc makes it run and not build it. Then looked at https://stackoverflow.com/questions/74382131/docker-net-the-command-could-not-be-loaded-while-dotnet-and-dll-file-are-pre which showed example of how to build, publish and then create an ENTRYPOINT.
- The image can now be created. Can see with `docker image ls`, and run / create with container with `docker run --rm marho/mychirp`
- Ran into issue when running server. It opened on `Now listening on: http://[::]:8080`. Fix: Needed to define `-p 8080:8080` in the command when running and `EXPOSE 8080` in the Dockerfile - like the exercises from this week.
- Can now run start the container with `docker run -p 8080:8080 marho/mychirp` and open `http://localhost:8080/`. Added my notes to the commit.

# Lecture 03
10/02: 11:35: Refactord Chirp to be named MiniTwit & Moved legacy code to own branch.


# Lecture 04
13/02: 11:45: Had problems with docker not being installed on VM on DigitalOcean. 
- Fixed with using the same commands we used to install docker in session02 PREP.md. Had problem with missing commandline flag `-y` but fixed.

13/02: 12:10: Fixed that `docker compose up`. Was not run from the right folder. That came with a new problem: the script would not recognize docker even though when manually ssh' into the VM, docker was installed.

14/02: 13:46: Fixed problem with docker not being recoginzed
- Explaned the problem to ChatGPT and it taught me that when a provisioner run (the script running after the server is created), it may not have updated the PATH, so docker is not recognised. It was therefore fixed by using two provisioner scripts, one for installing docker and one for running docker (each in their own shell).
- To spin up new Droplet, run: `vagrant up`. To destroy the droplet, run: `vagrant destroy` . If we make changes to MiniTwit and want it on the server, run: `vagrant rsync`, then `vagrant ssh`, `cd /vagrant`, and then `docker compose up -d --build`.

14/02: 14:25: Tested if database persisted when updating MiniTwit, it did not.
- Was because `rsync` was overwritting everything in /vagrant on the VM with the local folder - which meant the database too.
- The Vagrantfile now creates a `/minitwit/data` folder, that will contains the database file. Then updated the docker compose file, such that it is mapping the database file in the Docker container to `/minitwit/data`. 
- Current server: http://159.89.20.247:8080/

17/02 13:11
To run the Server with minimal API, from the itu-minitwit/src/MiniTwit.Web run `dotnet run`
The API can be observed on http://localhost:5221/swagger/index.html
To run the simulator against the API, find the simulator at BSc_lecture_notes/sessions/session_03/API_Spec and run `python3 minitwit_simulator.py http://localhost:5221/` (while the server is running)
To fit the 'minitwit_sim_api_test.py' tests to the application, set the BASE_URL at line 10 to 'http://127.0.0.1:5221', and out-comment lines 30 and 31. 
To run the tests against the api, run `pytest minitwit_sim_api_test.py` (while the server is running)
12/02: 11:00: Put database into folder. So we later can use this folder as our Docker Volume
12/02: 11:27:  Created docker compose file, such that we can persists out database. The docker container is linked to our /src/MiniTwit.Web/Data folder - so when we close and open the container the data persists. Can run it with `docker compose up`.
=======
# Lecture 04
12/02: 11:00: Put database into folder. So we later can use this folder as our Docker Volume
12/02: 11:27: Created docker compose file, such that we can persists out database. The docker container is linked to our /src/MiniTwit.Web/Data folder - so when we close and open the container the data persists. Can run it with `docker compose up`.

17/02 13:11: To run the Server with minimal API, from the itu-minitwit/src/MiniTwit.Web run `dotnet run`
- The API can be observed on http://localhost:5221/swagger/index.html
- To run the simulator against the API, find the simulator at BSc_lecture_notes/sessions/session_03/API_Spec and run `python3 minitwit_simulator.py http://localhost:5221/` (while the server is running)
- To fit the 'minitwit_sim_api_test.py' tests to the application, set the BASE_URL at line 10 to 'http://127.0.0.1:5221', and out-comment lines 30 and 31.
- To run the tests against the api, run `pytest minitwit_sim_api_test.py` (while the server is running)

20/02: 12:30: Setup our ci/cd pipeline (not tested yet, needs to be in main). 
- The general idea is as follows: We have the github workflow `cicd.yaml`, which is the general file that first creates the minitwit image that gets uploaded to Dockerhub (CI - Continuous Integration). Remember to setup secrets (look at our README.md or exercises for session04). The workflow then runs tests. This still needs to be setup. The workflow ssh into the server, and runs the `deploy.sh` script.
- The `deploy.sh` script pulls the latest image of our minitwit and runs docker compose, using the `docker-compose.yml`file, which works the same way as before, now it, instead of building the image, uses the image we put on Dockerhub.
- The Vagrantfile also changed. It not only syncs the `remote_files` directory, as it contains the `deploy.sh` and `docker-compose.yml` files.

20/02: 12:30: Setup our ci/cd pipeline (not tested yet, needs to be in main). 
- The general idea is as follows: We have the github workflow `cicd.yaml`, which is the general file that first creates the minitwit image that gets uploaded to Dockerhub (CI - Continuous Integration). Remember to setup secrets (look at our README.md or exercises for session04). The workflow then runs tests. This still needs to be setup. The workflow ssh into the server, and runs the `deploy.sh` script.
- The `deploy.sh` script pulls the latest image of our minitwit and runs docker compose, using the `docker-compose.yml`file, which works the same way as before, now it, instead of building the image, uses the image we put on Dockerhub.
- The Vagrantfile also changed. It not only syncs the `remote_files` directory, as it contains the `deploy.sh` and `docker-compose.yml` files.

21/02: 14:20: Added `build_and_test` and `release` workflows (taken from Chirp). The `build_and_test` will run on pushes to main or by triggering it manually (mostly for testing purposes - which i will do now). When completed and if it succeeds, it will trigger the `release` and `cdcd` workflows, creating a release and staring the CI/CD pipeline.

# Lecture 05

27/02: 17:30: Trying to fix our simulator API by adding response codes 
- Discovered we may have misintepreted what the terminal outputs of the minitwit_sim.py meant. They are giving us information on the http responses that doesn't match the expected http codes. Have refactored our simulator API endpoints in MiniTwit.Web/Api.cs to handle the appropriate response codes. Now when the simulator runs, no further output in the terminal appears when the simulator is run. 
- It is still unclear if this will have fixed the errors shown at http://64.226.108.122/status.html. We will probably need to deploy to find out for certain.


28/02: 12:30: Fixing Api-response branch so it passes the all the simulator api tests
- Failed two of the tests due to a type error concerning the response object to a GetFollowers request. Code returned the list of followers, but said list needed to be wrapped inside the FollowsResponse object from the API specification. Easy fix

28/02: 12:40: Fixed that the CI/CD pipeline and our release workflows no longer run on pull-requests, as they now check if the event was a 'push'.

28/02: 20:49: fixed permissions so workflows can readn and write to files and not on read-only mode

01/03: 12:00: Made our Vagrantfile idempotent. The provisioner script can be run with `vagrant provision`. 
- While doing it i looked at [https://stackoverflow.com/questions/592620/how-can-i-check-if-a-program-exists-from-a-bash-script](vscode-file://vscode-app/usr/share/code/resources/app/out/vs/code/electron-browser/workbench/workbench.html) and https://arslan.io/2019/07/03/how-to-write-idempotent-bash-scripts/.

# Lecture 06
03/03: We decided to use the database manger provided by DigitalOcean, more specifically Postgres

05/03 - edited (14/03): To migrate we needed to provide the database server with a postgres dump. However, our current database is sqlite, so it would create a sqlite dump, which we did:
- Copy sqlite db from server
``` scp root@209.38.230.113:/minitwit/data/chirp.db <location you want it to be put in>```
- make sqlite dump
```sqlite3 chirp.db .dump > minitwitdb.sql ```

- We create a postgres database locally to help with the dump
```createdb -U postgres tempdb```

- Then using PGloader, we load the sqlite dump into the postgres tempdb
```pgloader sqlite://chirp.db postgresql://postgres:miniDBTwit@localhost/tempd```

- Then we create the dump. The reason for --no-owner and --no-acl, is that we had a conflict between the user on digtal ocean, giving errors, because it was not marked as owner, the local postgres user was. Setting it to no owner, removed the conflicts.
```sudo -u postgres pg_dump --no-owner --no-acl tempdb > minitwitdb.sql```

- Then we upload the dump to the digital ocean database
```psql "postgresql://doadmin:...@db-postgresql-minitwit-do-user-33189704-0.f.db.ondigitalocean.com:25060/minitwitdb?sslmode=require"```

08/03: 12:00: Setup Promethues and Grafana to do visualization of our application. The idea is that we in out .NET MiniTwit application expose /metrics, which is out "in memory" current view of the system (or whatever we expose). We then use Promethues to scrape this data and safe it. Grafana then uses what Promethues scrape and visualize it.
- To expose more, see the MiniTwit.Infrastructure/Metrics.
- To Visualize it, go to grafana at localhost/3000 and find the dashboard, click edit, click add visualiztion. When you have added something copy the json file into /monitoring/grafana/dashboard/dashboard.json

09/03: 18:00: Ran into uses with the github actions not being able to find the dockerfiles - because of the context being setup wrong. Also forgot to create two new repositories on Dockerhub for the new images.


# Lecture 7

15/03 13:45: Research on which linter(s) and codeformatter to use. They will fail if they report issues and we will then have to fix (i included how to run locally)
- We use SonarQube Cloud (https://github.com/apps/sonarqubecloud). - We should look at if this is too much or not and if we can automate some of the fixes (it says there is ~600...)
- For the code formatter we will use [CSharpier](https://csharpier.com/docs/About), which has the following github for github actions:  [guibranco/github-csharpier-linter-action](https://github.com/guibranco/github-csharpier-linter-action). To use locally: `dotnet csharpier format .`
- For our Dockerfile(s) we will use  [hadolint/hadolint-action: GitHub action for Hadolint, A Dockerfile linting tool](https://github.com/hadolint/hadolint-action). To use locally first pull their docker image: `docker pull hadolint/hadolint` and then run `docker run --rm -i hadolint/hadolint < Dockerfile`, with 'Dockerfile' being our dockerfile.

# Lecture 8
20/03 10:45: Setup Alloy and Loki for logging 
- Copied loki.yml file from this week's exercise repo and added a config.alloy file instead of the promtail. The alloy file was based on this guide/tutorial: [Setting Up Grafana Loki and Alloy for Docker: A Practical Guide From My Recent Battle](https://kycha-blog.org/posts/practical-guide-grafana-alloy-loki-docker)
- modified the docker compose file to include containers for loki and alloy. Had some problems with the alloy container exitting immdiately, but this was due to the command flag `--config.file=/etc/loki/local-config.yaml` not being recognized by alloy. Instead the command should just be `run /etc/loki/loki.yml`

20/03 13:15: Replace alloy config file
- Was not satified with the structure of the logs, so I tried to use the converter tool on the promtail config file from the exercises. This generated a better base template for our alloy configuration, so I scraped the one from the online guide and replaced it with this instead. 


11/03: Updates for project
- To switch from Sqlite to postgres we install a postgres package in the Minitwit.Web project. Version 8.0.4, since we are usinge .Net 8.
```dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.4```
- We also added the connectionstring to github secrets, which for obvious reason im not going to show. We use this conncetionstring to deploy from gihub actions, as we have already been doing. Now it should connect to the database server. The connection string will point to our new database.

- We also updated the cicd workflow and docker compose, so they now hav an environment variable pointing to our connectionstring.

- Note: You can connect to the database if you have postgres installed and use following command:
```psql "postgresql://doadmin:<Our_password>@db-postgresql-minitwit-do-user-33189704-0.f.db.ondigitalocean.com:25060/minitwitdb?sslmode=require"```

17/03 15:43: Added our connectionstring to the postgres database to the server as an environment variable.
- Commands used: 
    edit evironment variables
    ```nano ~/.bashrc```  
    Insterted following into the file.
    ``` export ConnectionStrings__DefaultConnection="our_connectionstring"``` 
    Confirm
    ```source ~/.bashrc```

21/03 14:22: 
- fixed some issues with our backend relating to Likes. EFCore had trouble with converting ```Cheep.Likes.Count```into a correct query, giving us an error when we tried to access "top cheeps". We fixed this by editing our Cheep class to have the number of likes it has gotten (NrLikes), not just the list of those who have liked the cheep. This fixed the issue.

- 15:09: We also had an issue with people accessing their own timeline. This was due to some misplaced "awaits" (awaits within awaits) that led to conflicting executions, mainly being an issues with follows. 

- 15:41: We also had issues with some typeErrors with our ids which we had updated to longs instead of ints, missing some places. We also did a possible suboptimal fix to our GetAllLikes, where we fetch the all cheeps and filter them, based on wether or an author has liked them. This is suboptimal, and in a different scenario, not be a good solution.


05/04 12:06:
- Today we finally got out database working. This time we had a different approach; we first made it work locally. So we now create a local docker container with postgres and got it working with starting an empty database and filling it with our test data. After that, the next issue was fitting the old data to new the database / migrations and making that convertion. This is a 1-time only thing, so i did not need to be pretty. We got Claude AI to help us making some queries to convert and we got it working. The final step was to do this on the remote postgres database and then connection our production to it. 
- After switching to using postgres, we ran into a problem with running our python test. The problem was caused by using .Result instead og await. .Result was ok when we were using the sqlite .db database as it blocked the whole thread. But when using .Result on our containerized Postgres database this caused a race condition. 


# Lecture 9
24/03: 11:00: For now, we just install nginx & certbot directly on the server.
- Bought free domain from https://controlpanel.tech/servlet/ListAllOrdersServlet?formaction=listOrders. "Devbobs.tech"
- Pointed the nameservers to Digitalocean.
- Followed the given tutorial: https://github.com/itu-devops/BSc_lecture_notes/blob/master/sessions/session_09/TLSTutorial.md
- Ran into problem that dns providers could not see our name: https://dnschecker.org/#A/devbovs.tech. 
- Ran into a problem where Certbot could not give an certifacate because Let's Encrypt tries to reach us on port 80 (which we block).
- Could not figure out why devbobs.tech was not working. Asked ClaudeAI and found it our ip http://209.38.230.113/, do work, so Nginx does work with our IP. But the DNS does not work (yet, lets wait and see)

