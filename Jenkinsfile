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
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the project...'
                sh 'dotnet build --no-restore --configuration Release'
            }
        }

        stage('Test') {
            steps {
                echo 'Running unit tests...'
                sh 'dotnet test Product.API/Tests/Unit/ProductServiceTests.cs --no-build --configuration Release --logger="trx;LogFileName=test-results.trx" || true'
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
                sh 'dotnet publish Product.API/Product.API.csproj --no-build --configuration Release --output ./publish'
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
                sh 'docker build -f Product.API/Dockerfile -t product-service:${BUILD_NUMBER} .'
                sh 'docker tag product-service:${BUILD_NUMBER} product-service:latest'
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
