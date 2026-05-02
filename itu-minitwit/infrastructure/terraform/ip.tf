# https://developer.hashicorp.com/terraform/language/syntax/configuration
#resource "digitalocean_floating_ip" "public-ip" {
#  region = var.region
#}

#https://docs.digitalocean.com/reference/terraform/reference/resources/floating_ip_assignment/
resource "digitalocean_floating_ip_assignment" "public-ip" {
  ip_address = "IP"
  droplet_id = digitalocean_droplet.minitwit-swarm-leader.id
}

# expose a public_ip value - here our hardcoded value
output "public_ip" {
  value = "IP"
}
