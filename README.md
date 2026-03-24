# Devops_devBobs
DevOps, Software Evolution and Software Maintenance, BSc (Spring 2026) - Group "Devbobs"
> In this course, the students will discover all the software engineering activities that 
> take place after an initial software product is delivered or after a legacy system is taken over from a theoretical and practical perspective. Students (in groups)
> will take over such a system that is live and serving users, 
> refactor and migrate it to the languages and technologies of their liking. All subsequent DevOps, software evolution and software maintenance activities will be performed directly on the systems of the students.

# Link to MiniTwit
[link](http://209.38.230.113:8080/)

# Clone, deploy and setting up our CI/CD pipeline
The following explains how to go from cloning to deploying minitwit.

MiniTwit can be cloned with
```bash
$ git clone https://github.com/DevBobs-devops/Devbobs_MiniTwit.git
```

## Setting up SSH keys and DigitalOcean Droplet.
To deploy MiniTwit, make sure you have Vagrant installed, the [vagrant-digitalocean](https://www.digitalocean.com/community/tools/vagrant-digitalocean-2) plugin and:
1. To have a pair of SSH keys, if not follow [this tutorial](https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys-on-ubuntu-1804). Your ssh keys have to be in the directory.`~/.ssh/id_rsa`
2. Register at DigitalOcean
3. [Registered your public SSH key at DigitalOcean](https://www.digitalocean.com/docs/droplets/how-to/add-ssh-keys/to-account/).
4. Setup the two environment variables `$SSH_KEY_NAME` and `$DIGITAL_OCEAN_TOKEN` in `src/Vagrantfile`. 
  - `$SSH_KEY_NAME` is the name of the key you registered at Digitalocean at step 3.
  - `$DIGITAL_OCEAN_TOKEN` is the API token you get from DigitalOceanm, see: [tutorial](https://www.digitalocean.com/docs/api/create-personal-access-token/).

### Deploying a new VM
To deploy a new Droplet/VM on Digitalocean simply run:
```bash
$ cd src
$ vagrant up
```
The Vagrantfile creates a VM, see `itu-minitwit/Vagrantfile` and installs Docker.

To destroy the droplet, run: `vagrant destroy`.

## Setup Artifact Store
- First register at [Docker Hub](https://hub.docker.com/), which will be where we store the docker image of minitwit.
- Then setup the environment variables `DOCKER_USERNAME` and `DOCKER_PASSWORD`, as you did with your SSH key. These will be used to acces DockerHub. See `cicd.yaml`, `Vagrantfile` and `Deploy.sh`.
- Instead of using your actual DockerHub password, you can setup a [Access Token](https://docs.docker.com/security/access-tokens/) for DockerHub.
- Lastly, create a [repository](https://docs.docker.com/docker-hub/repos/create/) named `minitwitimage`, one named `minitwit-grafana`, one named `minitwit-alloy`, one named `minitwit-loki `and one named `minitwit-prometheus`.

## Configure Secrets on Github repository.
Setup the following secrets on github, see [here](https://docs.github.com/en/actions/how-tos/write-workflows/choose-what-workflows-do/use-secrets) for how.

  - `DOCKER_USERNAME` username for hub.docker.com
  - `DOCKER_PASSWORD` access token for username for hub.docker.com
  - `SSH_USER` the user as whom we will connect to the server at DigitalOcean, default is `root`
  - `SSH_KEY` the **private** SSH key we generated earlier (not the public key, if you followed the instructions it should be located at `~/.ssh/do_ssh_key`)
  - `SSH_HOST` the IP address of the server (or DNS name) we created on DigitalOcean, which you noted down earlier.

## Trigger the workflow
To trigger the workflow and start the pipeline, make a commit to the `main` branch, or [manually run the workflow](https://docs.github.com/en/actions/how-tos/manage-workflow-runs/manually-run-a-workflow).

To see the workflow, see `cicd.yaml`. 
- This will use the `Dockerfile` to create a docker image of minitwit, push it to Dockerhub, shh into the VM/Droplet and run the `deploy.sh` script, that runs the `docker-compose.yml`, which can be found in the directory `/remote_file`. 

# The notebook
To make work more visible, we have decided to create `notebook.md`. Here changes can be seen together with  **what went wrong**, **where** we found a **solution**.
