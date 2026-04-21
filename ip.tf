#resource "digitalocean_floating_ip_assignment" "public-ip" {
#  ip_address = digitalocean_floating_ip.public-ip.ip_address
#  droplet_id = digitalocean_droplet.minitwit-swarm-leader.id
#}

output "public_ip" {
  value = 134.199.190.79
}
