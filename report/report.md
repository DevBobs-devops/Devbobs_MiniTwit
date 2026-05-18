# System's Perspective
This section will give description and illustrations of the design and architecture of our Mini-twit system, dependencies and the current state of out system.

## Design and architecture of our ITU-MiniTwit systems.

# Process Perspective

## CI/CD Pipelines
The CI/CD pipeline consists of three stages. The pipeline can be seen in `/.github/workflows/`. All pipeliens are run on `ubuntu-24.04`

### Build and test
The build and test workflow is run on `push` to main and on `pull-requests` and consists of the following steps that run in parrallel:
1. **Code linter with Csharpier**: Checks the format of the code and reports. Does not automaticly apply the format.
2. **Dockerfile linter with Hadolint**: Checks the format of the dockerfile. 
3. **Build the application**: Builds the applications and runs test:
- Runs unit and intergreation test
- Runs test on the API / simulator endpoint.
- Runs E2E / Playwright tests

### CI/CD 
The CI/CD workflow runs on a succesfull Build and Test workflow and push to main. It consists of the steps:
1. **Dockerhub - CI**:
- Builds and pushes the `Minitwit`, `Grafana`, `Alloy`, `Loki` and `Promethues` docker images to Dockerhub.
2. **SSH and Deploy - CD**
- SSH into the node named leader in the swarm.
- Copy the stack file `/itu-minitwit/infrastructure/docker_swarm/stack/minitwit_stack.yml` to the node.
- Deploy to the swarm.

