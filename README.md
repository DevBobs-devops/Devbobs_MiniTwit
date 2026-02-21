# Devops_devBobs
DevOps, Software Evolution and Software Maintenance, BSc (Spring 2026) - Group "Devbobs"
> In this course, the students will discover all the software engineering activities that 
> take place after an initial software product is delivered or after a legacy system is taken over from a theoretical and practical perspective. Students (in groups)
> will take over such a system that is live and serving users, 
> refactor and migrate it to the languages and technologies of their liking. All subsequent DevOps, software evolution and software maintenance activities will be performed directly on the systems of the students.

# Clone, deploy and update MiniTwit
MiniTwit can be cloned with
```bash
$ git clone https://github.com/mortfb/Devops_devBobs.git
```

## Setting up Vagrant, keys & secrets
To deploy MiniTwit, make sure you have Vagrant installed, the [vagrant-digitalocean](https://www.digitalocean.com/community/tools/vagrant-digitalocean-2) plugin and:
1. To have a pair of SSH keys, if not follow [this tutorial](https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys-on-ubuntu-1804). Your ssh keys have to be in the directory.`~/.ssh/id_rsa`
2. Register at DigitalOcean
3. [Registered your public SSH key at DigitalOcean](https://www.digitalocean.com/docs/droplets/how-to/add-ssh-keys/to-account/).
4. Setup the two environment variables `$SSH_KEY_NAME` and `$DIGITAL_OCEAN_TOKEN` in `src/Vagrantfile`. 
  - `$SSH_KEY_NAME` is the name of the key you registered at Digitalocean at step 3.
  - `$DIGITAL_OCEAN_TOKEN` is the API token you get from DigitalOceanm, see: [tutorial](https://www.digitalocean.com/docs/api/create-personal-access-token/).

## Deploying a new VM
To deploy a new Droplet/VM on Digitalocean simply run:
```bash
$ cd src
$ vagrant up
```
The Vagrantfile creates a VM, see `src/Vagrantfile`, Installs Docker and starts MiniTwit as a docker container, see `src/Dockerfile` & `src/docker-compose.yml`. 

To destroy the droplet, run: `vagrant destroy` .

## Sync / Update MiniTwit
To sync changes to the Droplet, run:
```bash
$ cd /src
$ vagrant rsync
$ vagrant ssh
$ cd /vagrant
$ docker compose up -d --build
```

# The notebook
To make work more visible, we have decided to create `notebook.md`. Here changes can be seen together with  **what went wrong**, **where** we found a **solution**.