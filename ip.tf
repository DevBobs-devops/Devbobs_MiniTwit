# https://developer.hashicorp.com/terraform/language/syntax/configuration
#resource "digitalocean_floating_ip" "public-ip" {
#  region = var.region
#}

resource "digitalocean_floating_ip_assignment" "public-ip" {
  ip_address = 134.199.190.79
  droplet_id = digitalocean_droplet.minitwit-swarm-leader.id
}

output "public_ip" {
  value = 134.199.190.79
}
