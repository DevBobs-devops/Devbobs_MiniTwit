# Devops_devBobs
DevOps, Software Evolution and Software Maintenance, BSc (Spring 2026) - Group "Devbobs"
> In this course, the students will discover all the software engineering activities that 
> take place after an initial software product is delivered or after a legacy system is taken over from a theoretical and practical perspective. Students (in groups)
> will take over such a system that is live and serving users, 
> refactor and migrate it to the languages and technologies of their liking. All subsequent DevOps, software evolution and software maintenance activities will be performed directly on the systems of the students.

# Link to MiniTwit
A live hosted version of MiniTwit can be found at: 
[devbobs.tech](http://devbobs.tech/)

# Running it locally
To run it locally you need to have docker installed
You can then run `deploy_swarm_sh` to deploy a local swarm. see `/itu-minitwit/infrastructure/docker_swarm/local_stack/minitwit_stack.yml`. for details.

# Clone, deploy and setting up our CI/CD pipeline
The following explains how to go from cloning to deploying minitwit.

MiniTwit can be cloned with
```bash
$ git clone https://github.com/DevBobs-devops/Devbobs_MiniTwit.git
```

## Prerequesites
To deploy MiniTwit, make sure you have a digitalocean account and Docker installed.

## Setup
To get everything up and running, please do the following

### Install Terraform
Follow instructions from https://learn.hashicorp.com/tutorials/terraform/install-cli

### Generate ssh key
Now generate the ssh key that will be used to connect to the deployment

```bash
cd /itu-minitwit/infrastructure/terraform
mkdir ssh_key && ssh-keygen -t rsa -b 4096 -q -N '' -f ./ssh_key/terraform
```
### Setup secrets
For the rest of the setup be in the `/itu-minitwit/infrastructure/terraform` folder!

First create the secrets file. 
```bash
cp secrets_template secrets
```

You will now need to fill all fields out. Do the following:

#### Digital Ocean token
Go to https://cloud.digitalocean.com and find the API token page (https://cloud.digitalocean.com/account/api/) and generate a new API token.
- Give it "Full access" or limit it to your needs.
After creating it, copy the token and fill out the following line in the `secrets` file
```bash
export TF_VAR_do_token=
```

#### Digital Ocean Space
A digital ocean space will be used to store the terraform state file (https://developer.hashicorp.com/terraform/language/state)

Go to: https://cloud.digitalocean.com/spaces and click the blue "Create Bucket" button, choose the region closest to you.

Then copy the name of the bucket to the `secrets` file and fill out the <bucket_name>

```bash
export SPACE_NAME=<bucket_name>
```

You will now create a Space access key.
In the `Space Object Storage` under `Access Keys` click the blue `Create Access Key` button (https://cloud.digitalocean.com/spaces/access_keys).

Give it a name. This name should be set in the secrets file at <access_key_name>. Set the access to full or the access you want it to have.
After clicking `Create Access Key` save the secret key to <access_key_secret>

```bash
export AWS_ACCESS_KEY_ID=<access_key_name>
export AWS_SECRET_ACCESS_KEY=<access_key_secret>
```

#### Terraform state file
Create a terraform.tfstate file and set its path. For example `minitwit/terraform.tfstate`.

#### Floating IP
Create a floating ip. Follow: https://www.digitalocean.com/blog/floating-ips-start-architecting-your-applications-for-high-availability

This ip needs to be set in the `/ip.tf` file at <IP>

#### Setup Artifact Store
- First register at [Docker Hub](https://hub.docker.com/), which will be where we store the docker image of minitwit.
- The fill out the <docker_username> in the secrets file with your dockerhub username
- Instead of using your actual DockerHub password, you can setup a [Access Token](https://docs.docker.com/security/access-tokens/) for DockerHub.
- Lastly, create a [repository](https://docs.docker.com/docker-hub/repos/create/) named `minitwitimage`, one named `minitwit-grafana`, one named `minitwit-alloy`, one named `minitwit-loki `and one named `minitwit-prometheus`.

#### Domain
If you own a domain e.g. `Devbobs.tech`, make sure to point it at your floating ip on DigitalOcean and set it in `/itu-minitwit/infrastructure/docker_swarm/stack/minitwit_stack.yml` file in the minitwit service instead of `devbobs.tech`. If you do not own a domain, please outcomment this line.

## Deploying the infrastructure.
You can now deploy everything!

_note: if you have not yet pushed the five images to DockerHub you will have to do this. After deploying, this will be handled by the CI/CD pipeline._
-  `minitwitimage`, `minitwit-grafana`, `minitwit-alloy`, `minitwit-loki `, `minitwit-prometheus`

Do deploy the infrastructure run `./bootstrap.sh`.

This will take a while to run. When it is finished, you can now access MiniTwit with your floating ip.

In the terminal, some secrets will be written. These you will have setup on Github as secrets to use out CI/CD pipeline.

To take everything down, run `terraform destroy -auto-approve"`.

## Configure Secrets on Github repository.
Setup the following secrets on github, see [here](https://docs.github.com/en/actions/how-tos/write-workflows/choose-what-workflows-do/use-secrets) for how.

  - `DOCKER_USERNAME` username for hub.docker.com
  - `DOCKER_PASSWORD` access token for username for hub.docker.com
  - `SSH_USER` the user as whom we will connect to the server at DigitalOcean, default is `root`
  - `SSH_KEY` the **private** SSH key we generated earlier (not the public key, if you followed the instructions it should be located at `~/.ssh/do_ssh_key`)
  - `SSH_HOST` the IP address of the server (or DNS name) we created on DigitalOcean, which you noted down earlier.
  - `CONNECTION_STRING` is the connection string that was created to access your database.

## Trigger the workflow
To trigger the workflow and start the pipeline, make a commit to the `main` branch, or [manually run the workflow](https://docs.github.com/en/actions/how-tos/manage-workflow-runs/manually-run-a-workflow).

To see the workflow, see the files in  `.github/workflows`. 

# The notebook
To make work more visible, we have decided to create `notebook.md`. Here changes can be seen together with  **what went wrong**, **where** we found a **solution**.
