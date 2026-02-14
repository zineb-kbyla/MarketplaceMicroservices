pipeline {
    agent any

    options {
        timeout(time: 30, unit: 'MINUTES')
        timestamps()
    }

    environment {
        DOTNET_CLI_HOME = "${WORKSPACE}/.dotnet"
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = true
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code from GitHub...'
                git branch: 'master',
                    url: 'https://github.com/zineb-kbyla/MarketplaceMicroservices.git'
            }
        }

        stage('Restore') {
            steps {
                echo 'Restoring NuGet packages...'
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the project...'
                bat 'dotnet build --no-restore --configuration Release'
            }
        }

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                bat 'dotnet test Product.API/Product.API.csproj --no-build --configuration Release --logger="trx;LogFileName=test-results.trx"'
            }

            post {
                always {
                    junit allowEmptyResults: true, testResults: '**/test-results.trx'
                }
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing the application...'
                bat 'dotnet publish Product.API/Product.API.csproj --no-build --configuration Release --output ./publish'
            }

            post {
                success {
                    archiveArtifacts artifacts: 'publish/**', fingerprint: true
                }
            }
        }

        stage('Docker Build') {
            steps {
                echo 'Building Docker image...'
                bat 'docker build -f Product.API/Dockerfile -t product-service:%BUILD_NUMBER% .'
                bat 'docker tag product-service:%BUILD_NUMBER% product-service:latest'
            }
        }
    }

    post {
        always {
            echo 'Pipeline execution completed'
            cleanWs()
        }

        success {
            echo 'Pipeline succeeded ✓'
        }

        failure {
            echo 'Pipeline failed ✗'
        }
    }
}
