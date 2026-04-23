# inspired by: https://github.com/itu-devops/itu-minitwit-docker-swarm-teraform
#  _                _
# | | ___  __ _  __| | ___ _ __
# | |/ _ \/ _` |/ _` |/ _ \ '__|
# | |  __/ (_| | (_| |  __/ |
# |_|\___|\__,_|\__,_|\___|_|

# create cloud vm
resource "digitalocean_droplet" "minitwit-swarm-leader" {
  image = "docker-20-04" // ubuntu-22-04-x64
  name = "minitwit-swarm-leader"
  region = var.region
  size = "s-1vcpu-1gb"
  # add public ssh key so we can access the machine
  ssh_keys = [digitalocean_ssh_key.minitwit.fingerprint]

  # specify a ssh connection
  connection {
    user = "root"
    host = self.ipv4_address
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "2m"
  }

  provisioner "file" {
    source = "stack/minitwit_stack.yml"
    destination = "/root/minitwit_stack.yml"
  }

  provisioner "remote-exec" {
    inline = [
      # allow ports for docker swarm
      "ufw allow 2377/tcp",
      "ufw allow 7946",
      "ufw allow 4789/udp",
      # ports for apps
      "ufw allow 80",
      "ufw allow 8080",
      "ufw allow 8888",
      # SSH
      "ufw allow 22",

      # initialize docker swarm cluster
      "docker swarm init --advertise-addr ${self.ipv4_address}"
    ]
  }
}

resource "null_resource" "swarm-worker-token" {
  depends_on = [digitalocean_droplet.minitwit-swarm-leader]

  # save the worker join token
  provisioner "local-exec" {
    command = "ssh -o 'ConnectionAttempts 3600' -o 'StrictHostKeyChecking no' root@${digitalocean_droplet.minitwit-swarm-leader.ipv4_address} -i ssh_key/terraform 'docker swarm join-token worker -q' > temp/worker_token"
  }
}

resource "null_resource" "swarm-manager-token" {
  depends_on = [digitalocean_droplet.minitwit-swarm-leader]
  # save the manager join token
  provisioner "local-exec" {
    command = "ssh -o 'ConnectionAttempts 3600' -o 'StrictHostKeyChecking no' root@${digitalocean_droplet.minitwit-swarm-leader.ipv4_address} -i ssh_key/terraform 'docker swarm join-token manager -q' > temp/manager_token"
  }
}


#  _ __ ___   __ _ _ __   __ _  __ _  ___ _ __
# | '_ ` _ \ / _` | '_ \ / _` |/ _` |/ _ \ '__|
# | | | | | | (_| | | | | (_| | (_| |  __/ |
# |_| |_| |_|\__,_|_| |_|\__,_|\__, |\___|_|
#                              |___/

# create cloud vm
resource "digitalocean_droplet" "minitwit-swarm-manager" {
  # create managers after the leader
  depends_on = [null_resource.swarm-manager-token]

  # number of vms to create
  count = 2

  image = "docker-20-04"
  name = "minitwit-swarm-manager-${count.index}"
  region = var.region
  size = "s-1vcpu-1gb"
  # add public ssh key so we can access the machine
  ssh_keys = [digitalocean_ssh_key.minitwit.fingerprint]

  # specify a ssh connection
  connection {
    user = "root"
    host = self.ipv4_address
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "2m"
  }

  provisioner "file" {
    source = "temp/manager_token"
    destination = "/root/manager_token"
  }

  provisioner "remote-exec" {
    inline = [
      # allow ports for docker swarm
      "ufw allow 2377/tcp",
      "ufw allow 7946",
      "ufw allow 4789/udp",
      # ports for apps
      "ufw allow 80",
      "ufw allow 8080",
      "ufw allow 8888",
      # SSH
      "ufw allow 22",

      # join swarm cluster as managers
      "docker swarm join --token $(cat manager_token) ${digitalocean_droplet.minitwit-swarm-leader.ipv4_address}"
    ]
  }
}


#                     _
# __      _____  _ __| | _____ _ __
# \ \ /\ / / _ \| '__| |/ / _ \ '__|
#  \ V  V / (_) | |  |   <  __/ |
#   \_/\_/ \___/|_|  |_|\_\___|_|
#
# create cloud vm
resource "digitalocean_droplet" "minitwit-swarm-worker" {
  # create workers after the leader
  depends_on = [null_resource.swarm-worker-token]

  # number of vms to create
  count = 2

  image = "docker-20-04"
  name = "minitwit-swarm-worker-${count.index}"
  region = var.region
  size = "s-1vcpu-1gb"
  # add public ssh key so we can access the machine
  ssh_keys = [digitalocean_ssh_key.minitwit.fingerprint]

  # specify a ssh connection
  connection {
    user = "root"
    host = self.ipv4_address
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "2m"
  }

  provisioner "file" {
    source = "temp/worker_token"
    destination = "/root/worker_token"
  }

  provisioner "remote-exec" {
    inline = [
      # allow ports for docker swarm
      "ufw allow 2377/tcp",
      "ufw allow 7946",
      "ufw allow 4789/udp",
      # ports for apps
      "ufw allow 80",
      "ufw allow 8080",
      "ufw allow 8888",
      # SSH
      "ufw allow 22",

      # join swarm cluster as workers
      "docker swarm join --token $(cat worker_token) ${digitalocean_droplet.minitwit-swarm-leader.ipv4_address}"
    ]
  }
}

// DATABASE!
// https://registry.terraform.io/providers/digitalocean/digitalocean/latest/docs/resources/database_cluster
resource "digitalocean_database_cluster" "postgres" {
  name       = "postgres-cluster"
  engine     = "pg"
  version    = "15"
  size       = "db-s-1vcpu-1gb"
  region     = "nyc1"
  node_count = 1
}

output "minitwit-swarm-leader-ip-address" {
  value = digitalocean_droplet.minitwit-swarm-leader.ipv4_address
}

output "minitwit-swarm-manager-ip-address" {
  value = digitalocean_droplet.minitwit-swarm-manager.*.ipv4_address
}

output "minitwit-swarm-worker-ip-address" {
  value = digitalocean_droplet.minitwit-swarm-worker.*.ipv4_address
}

// https://www.digitalocean.com/community/questions/how-to-retrieve-database-credentials-and-certificate-in-terraform
// we probably wanna use this somehow?
output "ConnectionStrings__DefaultConnection" {
  value = "postgresql://${digitalocean_database_user.db-user.name}:${digitalocean_database_user.db-user.password}@${data.digitalocean_database_cluster.db-cluster.host}/${resource.digitalocean_database_db.db.name}?sslmode=require"
  sensitive = true
}