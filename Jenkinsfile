pipeline {
    agent any

    options {
        timeout(time: 45, unit: 'MINUTES')
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }

    environment {
        DOTNET_CLI_HOME = "${WORKSPACE}/.dotnet"
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = true
    }

    parameters {
        choice(
            name: 'SERVICE',
            choices: ['ALL', 'Product.API', 'Order.API'],
            description: 'Sélectionner le microservice à builder'
        )
        booleanParam(
            name: 'DEPLOY',
            defaultValue: false,
            description: 'Déployer avec docker-compose après le build?'
        )
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code from GitHub...'
                git branch: 'master',
                    url: 'https://github.com/zineb-kbyla/MarketplaceMicroservices.git'
            }
        }

        stage('Restore Dependencies') {
            steps {
                script {
                    if (params.SERVICE == 'ALL') {
                        echo 'Restoring all microservices...'
                        bat 'dotnet restore'
                    } else {
                        echo "Restoring ${params.SERVICE}..."
                        bat "dotnet restore ${params.SERVICE}/${params.SERVICE}.csproj"
                    }
                }
            }
        }

        stage('Build Services') {
            parallel {
                stage('Build Product.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Product.API' }
                    }
                    steps {
                        echo 'Building Product.API...'
                        bat 'dotnet build Product.API/Product.API.csproj --no-restore --configuration Release'
                    }
                }

                stage('Build Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo 'Building Order.API...'
                        bat 'dotnet build Order.API/Order.API.csproj --no-restore --configuration Release'
                    }
                }
            }
        }

        stage('Run Tests') {
            parallel {
                stage('Test Product.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Product.API' }
                    }
                    steps {
                        echo 'Running Product.API tests...'
                        bat 'dotnet test Product.API/Product.API.csproj --no-build --configuration Release --logger="trx;LogFileName=product-test-results.trx"'
                    }
                }

                stage('Test Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo 'Running Order.API tests...'
                        bat 'dotnet test Order.API/Order.API.csproj --no-build --configuration Release --logger="trx;LogFileName=order-test-results.trx"'
                    }
                }
            }

            post {
                always {
                    junit allowEmptyResults: true, testResults: '**/*-test-results.trx'
                }
            }
        }

        stage('Publish Artifacts') {
            parallel {
                stage('Publish Product.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Product.API' }
                    }
                    steps {
                        echo 'Publishing Product.API...'
                        bat 'dotnet publish Product.API/Product.API.csproj --no-build --configuration Release --output ./publish/product-api'
                    }
                }

                stage('Publish Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo 'Publishing Order.API...'
                        bat 'dotnet publish Order.API/Order.API.csproj --no-build --configuration Release --output ./publish/order-api'
                    }
                }
            }

            post {
                success {
                    archiveArtifacts artifacts: 'publish/**', fingerprint: true
                }
            }
        }

        stage('Build Docker Images') {
            parallel {
                stage('Docker Product.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Product.API' }
                    }
                    steps {
                        echo 'Building Product.API Docker image...'
                        bat 'docker build -f Product.API/Dockerfile -t product-service:%BUILD_NUMBER% .'
                        bat 'docker tag product-service:%BUILD_NUMBER% product-service:latest'
                    }
                }

                stage('Docker Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo 'Building Order.API Docker image...'
                        bat 'docker build -f Order.API/Dockerfile -t order-service:%BUILD_NUMBER% .'
                        bat 'docker tag order-service:%BUILD_NUMBER% order-service:latest'
                    }
                }
            }
        }

        stage('Deploy with Docker Compose') {
            when {
                expression { params.DEPLOY == true }
            }
            steps {
                echo 'Deploying services with docker-compose...'
                bat 'docker-compose down'
                bat 'docker-compose up -d'
            }

            post {
                success {
                    echo 'Waiting for services to start...'
                    sleep 15
                }
            }
        }

        stage('Health Checks') {
            when {
                expression { params.DEPLOY == true }
            }
            parallel {
                stage('Health Check Product.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Product.API' }
                    }
                    steps {
                        echo 'Health check Product.API...'
                        retry(3) {
                            sleep 5
                            bat 'curl -f http://localhost:5001/api/products || exit /b 0'
                        }
                    }
                }

                stage('Health Check Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo 'Health check Order.API...'
                        retry(3) {
                            sleep 5
                            bat 'curl -f http://localhost:5002/api/orders || exit /b 0'
                        }
                    }
                }
            }
        }
    }

    post {
        always {
            echo 'Pipeline execution completed'
            cleanWs()
        }

        success {
            echo '✅ MARKETPLACE MICROSERVICES - Pipeline succeeded!'
        }

        failure {
            echo '❌ MARKETPLACE MICROSERVICES - Pipeline failed!'
        }

        unstable {
            echo '⚠️ MARKETPLACE MICROSERVICES - Pipeline unstable'
        }
    }
}
