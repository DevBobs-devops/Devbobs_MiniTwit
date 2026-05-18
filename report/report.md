# System's Perspective
This section will give description and illustrations of the design and architecture of our Mini-twit system, dependencies and the current state of out system.

## Design and architecture of our ITU-MiniTwit systems.

# Process Perspective

## CI/CD Pipelines
The CI/CD pipeline consists of three stages. The pipeline can be seen in `/.github/workflows/`. All pipeliens are run on `ubuntu-24.04`

### Build and test
The build and test workflow is run on `push` to main and on `pull-requests` and consists of the following steps:
1. **Code linter with Csharpier**: Checks the format of the code and reports. Does not automaticly apply the format.
2. **Dockerfile linter with Hadolint**: Checks the format of the dockerfile. 
3. **Build the application**: Builds the applications and runs test:
- Runs unit and intergreation test
- Runs the API / simulator as a test
- Runs E2E / Playwright tests

### CI/CD 
