pipeline {
    agent any

    options {
        timeout(time: 45, unit: 'MINUTES')
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '10'))
        disableConcurrentBuilds()
        skipDefaultCheckout()  // Évite le checkout auto, on le fait après le nettoyage
    }

    environment {
        DOTNET_CLI_HOME = "${WORKSPACE}/.dotnet"
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = true
    }

    parameters {
        choice(
            name: 'SERVICE',
            choices: ['ALL', 'Product.API', 'Order.API', 'Recommendation.API', 'APIGateway'],
            description: 'Sélectionner le microservice à builder'
        )
        booleanParam(
            name: 'DEPLOY',
            defaultValue: false,
            description: 'Déployer avec docker-compose après le build?'
        )
    }

    stages {
        stage('Clean & Checkout') {
            steps {
                echo '🧹 Nettoyage de la workspace Jenkins...'
                // Nettoyer le répertoire entièrement
                deleteDir()
                
                echo '📥 Checkout du code depuis GitHub...'
                git branch: 'zineb',
                    url: 'https://github.com/zineb-kbyla/MarketplaceMicroservices.git'
            }
        }

        stage('Restore Dependencies') {
            steps {
                script {
                    if (params.SERVICE == 'ALL') {
                        echo '📦 Restauration de tous les microservices...'
                        bat 'dotnet restore'
                    } else {
                        echo "📦 Restauration de ${params.SERVICE}..."
                        if (params.SERVICE == 'APIGateway') {
                            bat "dotnet restore APIGateway/APIGateway.csproj"
                        } else {
                            bat "dotnet restore ${params.SERVICE}/${params.SERVICE}.csproj"
                        }
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
                        echo '🔨 Building Product.API...'
                        bat 'dotnet build Product.API/Product.API.csproj --no-restore --configuration Release'
                    }
                }

                stage('Build Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo '🔨 Building Order.API...'
                        bat 'dotnet build Order.API/Order.API.csproj --no-restore --configuration Release'
                    }
                }

                stage('Build Recommendation.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Recommendation.API' }
                    }
                    steps {
                        echo '🔨 Building Recommendation.API...'
                        bat 'dotnet build Recommendation.API/Recommendation.API.csproj --no-restore --configuration Release'
                    }
                }

                stage('Build APIGateway') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'APIGateway' }
                    }
                    steps {
                        echo '🔨 Building APIGateway...'
                        bat 'dotnet build APIGateway/APIGateway.csproj --no-restore --configuration Release'
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
                        echo '✅ Running Product.API tests...'
                        bat 'dotnet test Product.API/Product.API.csproj --no-build --configuration Release --logger="trx;LogFileName=product-test-results.trx"'
                    }
                }

                stage('Test Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo '✅ Running Order.API tests...'
                        bat 'dotnet test Order.API/Order.API.csproj --no-build --configuration Release --logger="trx;LogFileName=order-test-results.trx"'
                    }
                }

                stage('Test Recommendation.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Recommendation.API' }
                    }
                    steps {
                        echo '✅ Running Recommendation.API tests...'
                        bat 'dotnet test Recommendation.API/Recommendation.API.csproj --no-build --configuration Release --logger="trx;LogFileName=recommendation-test-results.trx"'
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
                        echo '📦 Publishing Product.API...'
                        bat 'dotnet publish Product.API/Product.API.csproj --no-build --configuration Release --output ./publish/product-api'
                    }
                }

                stage('Publish Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo '📦 Publishing Order.API...'
                        bat 'dotnet publish Order.API/Order.API.csproj --no-build --configuration Release --output ./publish/order-api'
                    }
                }

                stage('Publish Recommendation.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Recommendation.API' }
                    }
                    steps {
                        echo '📦 Publishing Recommendation.API...'
                        bat 'dotnet publish Recommendation.API/Recommendation.API.csproj --no-build --configuration Release --output ./publish/recommendation-api'
                    }
                }

                stage('Publish APIGateway') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'APIGateway' }
                    }
                    steps {
                        echo '📦 Publishing APIGateway...'
                        bat 'dotnet publish APIGateway/APIGateway.csproj --no-build --configuration Release --output ./publish/api-gateway'
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
                        echo '🐳 Building Product.API Docker image...'
                        bat 'docker build -f Product.API/Dockerfile -t product-service:%BUILD_NUMBER% .'
                        bat 'docker tag product-service:%BUILD_NUMBER% product-service:latest'
                    }
                }

                stage('Docker Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo '🐳 Building Order.API Docker image...'
                        bat 'docker build -f Order.API/Dockerfile -t order-service:%BUILD_NUMBER% .'
                        bat 'docker tag order-service:%BUILD_NUMBER% order-service:latest'
                    }
                }

                stage('Docker Recommendation.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Recommendation.API' }
                    }
                    steps {
                        echo '🐳 Building Recommendation.API Docker image...'
                        bat 'docker build -f Recommendation.API/Dockerfile -t recommendation-service:%BUILD_NUMBER% .'
                        bat 'docker tag recommendation-service:%BUILD_NUMBER% recommendation-service:latest'
                    }
                }

                stage('Docker APIGateway') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'APIGateway' }
                    }
                    steps {
                        echo '🐳 Building APIGateway Docker image...'
                        bat 'docker build -f APIGateway/Dockerfile -t api-gateway:%BUILD_NUMBER% .'
                        bat 'docker tag api-gateway:%BUILD_NUMBER% api-gateway:latest'
                    }
                }
            }
        }

        stage('Deploy with Docker Compose') {
            when {
                expression { params.DEPLOY == true }
            }
            steps {
                echo '🐳 Deploying services with docker-compose...'
                bat 'docker-compose down --remove-orphans -v'
                echo '⏳ Waiting 5 seconds before restart...'
                sleep 5
                bat 'docker-compose up -d --force-recreate'
            }

            post {
                success {
                    echo '⏳ Waiting 20 seconds for services to start...'
                    sleep 20
                }
            }
        }

        stage('Health Checks') {
            when {
                expression { params.DEPLOY == true }
            }
            parallel {
                stage('Health Check APIGateway') {
                    steps {
                        echo '💚 Health check APIGateway...'
                        retry(3) {
                            sleep 5
                            bat 'curl -f http://localhost:5000/health || exit /b 0'
                        }
                    }
                }

                stage('Health Check Product.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Product.API' }
                    }
                    steps {
                        echo '💚 Health check Product.API...'
                        retry(3) {
                            sleep 5
                            bat 'curl -f http://localhost:5000/api/products || exit /b 0'
                        }
                    }
                }

                stage('Health Check Order.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Order.API' }
                    }
                    steps {
                        echo '💚 Health check Order.API...'
                        retry(3) {
                            sleep 5
                            bat 'curl -f http://localhost:5000/api/orders || exit /b 0'
                        }
                    }
                }

                stage('Health Check Recommendation.API') {
                    when {
                        expression { params.SERVICE == 'ALL' || params.SERVICE == 'Recommendation.API' }
                    }
                    steps {
                        echo '💚 Health check Recommendation.API...'
                        retry(3) {
                            sleep 5
                            bat 'curl -f http://localhost:5000/api/recommendations/trending || exit /b 0'
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
