# Devbobs's Notebook 
**Team:** Devbobs  
**Contributors:** Marius, Morten, Jonas & Laura

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
12/02: 11:27:  Created docker compose file, such that we can persists out database. The docker container is linked to our /src/MiniTwit.Web/Data folder - so when we close and open the container the data persists. Can run it with `docker compose up`.

17/02 13:11
To run the Server with minimal API, from the itu-minitwit/src/MiniTwit.Web run `dotnet run`
The API can be observed on http://localhost:5221/swagger/index.html
To run the simulator against the API, find the simulator at BSc_lecture_notes/sessions/session_03/API_Spec and run `python3 minitwit_simulator.py http://localhost:5221/` (while the server is running)
To fit the 'minitwit_sim_api_test.py' tests to the application, set the BASE_URL at line 10 to 'http://127.0.0.1:5221', and out-comment lines 30 and 31.
To run the tests against the api, run `pytest minitwit_sim_api_test.py` (while the server is running)

20/02: 12:30: Setup our ci/cd pipeline (not tested yet, needs to be in main). 
- The general idea is as follows: We have the github workflow `cicd.yaml`, which is the general file that first creates the minitwit image that gets uploaded to Dockerhub (CI - Continuous Integration). Remember to setup secrets (look at our README.md or exercises for session04). The workflow then runs tests. This still needs to be setup. The workflow ssh into the server, and runs the `deploy.sh` script.
- The `deploy.sh` script pulls the latest image of our minitwit and runs docker compose, using the `docker-compose.yml`file, which works the same way as before, now it, instead of building the image, uses the image we put on Dockerhub.
- The Vagrantfile also changed. It not only syncs the `remote_files` directory, as it contains the `deploy.sh` and `docker-compose.yml` files.

20/02: 12:30: Setup our ci/cd pipeline (not tested yet, needs to be in main). 
- The general idea is as follows: We have the github workflow `cicd.yaml`, which is the general file that first creates the minitwit image that gets uploaded to Dockerhub (CI - Continuous Integration). Remember to setup secrets (look at our README.md or exercises for session04). The workflow then runs tests. This still needs to be setup. The workflow ssh into the server, and runs the `deploy.sh` script.
- The `deploy.sh` script pulls the latest image of our minitwit and runs docker compose, using the `docker-compose.yml`file, which works the same way as before, now it, instead of building the image, uses the image we put on Dockerhub.
- The Vagrantfile also changed. It not only syncs the `remote_files` directory, as it contains the `deploy.sh` and `docker-compose.yml` files.

21/02: 14:20: Added `build_and_test` and `release` workflows (taken from Chirp). The `build_and_test` will run on pushes to main or by triggering it manually (mostly for testing purposes - which i will do now). When completed and if it succeeds, it will trigger the `release` and `cdcd` workflows, creating a release and staring the CI/CD pipeline.

27/02: 17:30: Trying to fix our simulator API by adding response codes 
- Discovered we may have misintepreted what the terminal outputs of the minitwit_sim.py meant. They are giving us information on the http responses that doesn't match the expected http codes. Have refactored our simulator API endpoints in MiniTwit.Web/Api.cs to handle the appropriate response codes. Now when the simulator runs, no further output in the terminal appears when the simulator is run. 
- It is still unclear if this will have fixed the errors shown at http://64.226.108.122/status.html. We will probably need to deploy to find out for certain.