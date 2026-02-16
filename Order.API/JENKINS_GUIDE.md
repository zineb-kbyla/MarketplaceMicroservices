# 🔧 Jenkins CI/CD - Order.API

Guide complet pour configurer et utiliser Jenkins avec le microservice Order.API.

---

## 📋 Table des matières

1. [Prérequis](#prérequis)
2. [Configuration Jenkins](#configuration-jenkins)
3. [Pipeline Order.API](#pipeline-orderapi)
4. [Pipeline Multi-Services](#pipeline-multi-services)
5. [Exécution des pipelines](#exécution-des-pipelines)
6. [Troubleshooting](#troubleshooting)

---

## 🎯 Prérequis

### Logiciels requis

- ✅ **Jenkins** 2.400+ installé et configuré
- ✅ **.NET SDK 10.0** installé sur l'agent Jenkins
- ✅ **Docker Desktop** installé et démarré
- ✅ **Git** configuré
- ✅ Accès au repository GitHub

### Plugins Jenkins nécessaires

Installer les plugins suivants via **Manage Jenkins → Plugins**:

```
- Git Plugin
- Pipeline Plugin
- Docker Pipeline Plugin
- JUnit Plugin
- Workspace Cleanup Plugin
- Timestamper Plugin
- Build Timeout Plugin
```

### Vérification de l'environnement

Sur la machine Jenkins:

```powershell
# Vérifier .NET SDK
dotnet --version
# Attendu: 10.0.x ou supérieur

# Vérifier Docker
docker --version
docker-compose --version

# Vérifier Git
git --version

# Vérifier curl (pour health checks)
curl --version
```

---

## 🔧 Configuration Jenkins

### Étape 1: Créer un nouveau Job pour Order.API

1. **Dashboard Jenkins** → **New Item**
2. Nom: `Order.API-Pipeline`
3. Type: **Pipeline**
4. Cliquer sur **OK**

### Étape 2: Configuration du Job

#### General

- ✅ **Description**: `Pipeline CI/CD pour le microservice Order.API`
- ✅ **Discard old builds**: 
  - Strategy: Log Rotation
  - Days to keep builds: `30`
  - Max # of builds to keep: `10`

#### Build Triggers

Choisir selon vos besoins:

- ✅ **Poll SCM**: `H/5 * * * *` (vérifie Git toutes les 5 minutes)
- ✅ **GitHub hook trigger** (si webhook configuré)
- ⬜ **Build periodically**: `H 2 * * *` (build quotidien à 2h du matin)

#### Pipeline Configuration

- **Definition**: `Pipeline script from SCM`
- **SCM**: `Git`
- **Repository URL**: `https://github.com/zineb-kbyla/MarketplaceMicroservices.git`
- **Branch**: `*/master`
- **Script Path**: `Order.API/Jenkinsfile`

Cliquer sur **Save**.

---

## 📦 Pipeline Order.API

Le fichier `Order.API/Jenkinsfile` contient le pipeline spécifique pour Order.API.

### Stages du pipeline

| Stage | Description | Durée estimée |
|-------|-------------|---------------|
| **Checkout** | Clone le repository Git | ~10s |
| **Restore Dependencies** | Restore les packages NuGet | ~30s |
| **Build** | Compile Order.API | ~45s |
| **Run Unit Tests** | Exécute les tests unitaires | ~20s |
| **Code Quality Analysis** | Analyse de la qualité du code | ~15s |
| **Publish Artifacts** | Publie les artefacts .NET | ~30s |
| **Build Docker Image** | Construit l'image Docker | ~1m 30s |
| **Push Docker Image** | Pousse l'image vers un registry | ~30s |
| **Deploy to Staging** | Déploie avec docker-compose | ~20s |
| **Health Check** | Vérifie que l'API répond | ~15s |

**Durée totale estimée:** ~5 minutes

### Variables d'environnement

```groovy
SERVICE_NAME = 'Order.API'
DOCKER_IMAGE_NAME = 'order-service'
DOTNET_CLI_HOME = "${WORKSPACE}/.dotnet"
DOTNET_SKIP_FIRST_TIME_EXPERIENCE = true
```

### Commandes principales

```bash
# Restore
dotnet restore Order.API/Order.API.csproj

# Build
dotnet build Order.API/Order.API.csproj --configuration Release

# Test
dotnet test Order.API/Order.API.csproj --logger="trx;LogFileName=order-test-results.trx"

# Publish
dotnet publish Order.API/Order.API.csproj --output ./publish/order-api

# Docker Build
docker build -f Order.API/Dockerfile -t order-service:%BUILD_NUMBER% .

# Docker Deploy
docker-compose up -d order-api

# Health Check
curl -f http://localhost:5002/api/orders
```

---

## 🚀 Pipeline Multi-Services

Le fichier racine `Jenkinsfile` permet de builder **Product.API** et **Order.API** simultanément.

### Fonctionnalités

- ✅ **Build paramétré** : Choisir quel service builder
- ✅ **Builds parallèles** : Product.API et Order.API en parallèle
- ✅ **Tests parallèles** : Exécution simultanée des tests
- ✅ **Déploiement optionnel** : Paramètre `DEPLOY` pour docker-compose
- ✅ **Health checks** : Vérification automatique des APIs

### Paramètres

| Paramètre | Type | Options | Description |
|-----------|------|---------|-------------|
| `SERVICE` | Choice | ALL, Product.API, Order.API | Microservice à builder |
| `DEPLOY` | Boolean | true/false | Déployer avec docker-compose? |

### Exemples d'utilisation

#### 1. Builder tous les services sans déployer

```
SERVICE = ALL
DEPLOY = false
```

#### 2. Builder uniquement Order.API et déployer

```
SERVICE = Order.API
DEPLOY = true
```

#### 3. Builder tous les services et déployer

```
SERVICE = ALL
DEPLOY = true
```

---

## ▶️ Exécution des pipelines

### Option 1: Build manuel (Jenkins UI)

1. **Dashboard** → **Order.API-Pipeline**
2. Cliquer sur **Build Now** (ou **Build with Parameters** si configuré)
3. Suivre les logs en temps réel

### Option 2: Build avec paramètres

1. **Dashboard** → **Marketplace-Pipeline** (pipeline multi-services)
2. Cliquer sur **Build with Parameters**
3. Sélectionner:
   - `SERVICE`: `Order.API`
   - `DEPLOY`: `✅` (coché)
4. Cliquer sur **Build**

### Option 3: Build automatique (Git Push)

Si webhook GitHub configuré:

```powershell
# Faire des modifications dans Order.API
git add .
git commit -m "feat: add new order feature"
git push origin master

# Jenkins détecte le push et lance automatiquement le build
```

### Option 4: Build programmé (Cron)

Si configuré avec **Build periodically**:

```
# Tous les jours à 2h du matin
H 2 * * *

# Tous les lundis à 8h
0 8 * * 1

# Toutes les heures
H * * * *
```

---

## 📊 Monitoring et Logs

### Voir les logs du build

1. **Dashboard** → **Order.API-Pipeline**
2. Cliquer sur le numéro du build (ex: `#15`)
3. **Console Output** pour voir les logs complets

### Voir les artefacts

1. **Dashboard** → **Order.API-Pipeline** → **Build #15**
2. **Build Artifacts** → Télécharger `publish/order-api/**`

### Voir les résultats des tests

1. **Dashboard** → **Order.API-Pipeline** → **Build #15**
2. **Test Results** → Voir les tests réussis/échoués

### Voir les images Docker

Sur l'agent Jenkins:

```powershell
# Voir toutes les images
docker images | findstr order-service

# Sortie attendue:
# order-service   15       abc123def456   2 hours ago   250MB
# order-service   latest   abc123def456   2 hours ago   250MB
```

---

## 🐛 Troubleshooting

### ❌ Erreur: "dotnet: command not found"

**Cause:** .NET SDK n'est pas installé ou pas dans le PATH

**Solution:**

```powershell
# Vérifier l'installation
dotnet --version

# Si non installé, télécharger depuis:
# https://dotnet.microsoft.com/download/dotnet/10.0

# Ajouter au PATH dans Jenkins:
# Manage Jenkins → System → Global properties → Environment variables
# Name: PATH
# Value: C:\Program Files\dotnet;${PATH}
```

---

### ❌ Erreur: "docker: command not found"

**Cause:** Docker n'est pas installé ou Docker Desktop n'est pas démarré

**Solution:**

```powershell
# Vérifier Docker Desktop
docker ps

# Si erreur, démarrer Docker Desktop manuellement

# Ajouter Docker au PATH Jenkins si nécessaire:
# Manage Jenkins → System → Global properties → Environment variables
# Name: PATH
# Value: C:\Program Files\Docker\Docker\resources\bin;${PATH}
```

---

### ❌ Erreur: "Connection refused" sur health check

**Cause:** L'API n'est pas encore démarrée ou le port est bloqué

**Solution 1: Augmenter le délai**

Dans `Jenkinsfile`, modifier:

```groovy
stage('Health Check') {
    steps {
        retry(5) {  // Au lieu de 3
            sleep 15  // Au lieu de 10
            bat 'curl -f http://localhost:5002/api/orders || exit 1'
        }
    }
}
```

**Solution 2: Vérifier que le service est UP**

```powershell
# Sur l'agent Jenkins
docker-compose ps order-api

# Voir les logs
docker-compose logs order-api
```

---

### ❌ Erreur: "Test results not found"

**Cause:** Les tests n'ont pas généré de fichier `.trx`

**Solution:**

Vérifier que le projet a des tests:

```powershell
# Vérifier la structure
dir Order.API\Tests

# Si pas de tests, le stage échouera
# Modifier le Jenkinsfile pour permettre des résultats vides:
junit allowEmptyResults: true, testResults: '**/*-test-results.trx'
```

---

### ❌ Erreur: "Workspace cleanup failed"

**Cause:** Fichiers verrouillés par un processus

**Solution:**

```groovy
post {
    always {
        script {
            try {
                cleanWs()
            } catch (Exception e) {
                echo "Cleanup failed: ${e.message}"
                // Continuer sans nettoyer
            }
        }
    }
}
```

---

### ❌ Erreur: "Docker build failed - no such file"

**Cause:** Le contexte de build Docker est incorrect

**Solution:**

Vérifier le Dockerfile:

```dockerfile
# ❌ Incorrect
COPY ["Order.API.csproj", "Order.API/"]

# ✅ Correct (car le build est fait depuis la racine)
COPY ["Order.API/Order.API.csproj", "Order.API/"]
```

Vérifier la commande Docker build:

```bash
# ❌ Incorrect
docker build -f Order.API/Dockerfile .

# ✅ Correct (le contexte doit être la racine)
docker build -f Order.API/Dockerfile -t order-service .
```

---

## 📈 Bonnes pratiques

### 1. Utiliser des stages parallèles

Pour builder plusieurs services:

```groovy
stage('Build Services') {
    parallel {
        stage('Build Product.API') { ... }
        stage('Build Order.API') { ... }
    }
}
```

### 2. Archiver les artefacts

```groovy
post {
    success {
        archiveArtifacts artifacts: 'publish/**', fingerprint: true
    }
}
```

### 3. Gérer les timeouts

```groovy
options {
    timeout(time: 30, unit: 'MINUTES')
}
```

### 4. Nettoyer le workspace

```groovy
post {
    always {
        cleanWs()
    }
}
```

### 5. Notifications

Ajouter des notifications Slack/Email:

```groovy
post {
    success {
        slackSend color: 'good', message: "Order.API Build #${BUILD_NUMBER} succeeded"
    }
    failure {
        mail to: 'team@example.com',
             subject: "Order.API Build #${BUILD_NUMBER} failed",
             body: "Check console output at ${BUILD_URL}"
    }
}
```

---

## 🔐 Configuration avancée

### Push vers Docker Registry

Pour pousser vers Docker Hub ou un registry privé:

```groovy
stage('Push Docker Image') {
    steps {
        withCredentials([
            usernamePassword(
                credentialsId: 'docker-hub-credentials',
                usernameVariable: 'DOCKER_USERNAME',
                passwordVariable: 'DOCKER_PASSWORD'
            )
        ]) {
            bat "docker login -u %DOCKER_USERNAME% -p %DOCKER_PASSWORD%"
            bat "docker push order-service:%BUILD_NUMBER%"
            bat "docker push order-service:latest"
        }
    }
}
```

### SonarQube Integration

Ajouter l'analyse de code:

```groovy
stage('SonarQube Analysis') {
    steps {
        withSonarQubeEnv('SonarQube') {
            bat 'dotnet sonarscanner begin /k:"Order.API"'
            bat 'dotnet build Order.API/Order.API.csproj'
            bat 'dotnet sonarscanner end'
        }
    }
}
```

---

## 📚 Ressources complémentaires

- **Dockerfile**: Voir `Order.API/Dockerfile`
- **Docker Commands**: Voir `DOCKER_COMMANDS.md`
- **Postman Tests**: Voir `POSTMAN_TESTS.md`
- **Architecture**: Voir `../ARCHITECTURE.md`

---

## ✅ Checklist de configuration Jenkins

- [ ] Jenkins installé et accessible
- [ ] Plugins installés (Git, Pipeline, Docker, JUnit)
- [ ] .NET SDK 10.0 installé sur l'agent Jenkins
- [ ] Docker Desktop installé et démarré
- [ ] Repository Git accessible
- [ ] Job `Order.API-Pipeline` créé
- [ ] Jenkinsfile présent dans `Order.API/Jenkinsfile`
- [ ] Premier build réussi
- [ ] Health check fonctionnel sur http://localhost:5002/api/orders
- [ ] Docker image créée: `order-service:latest`

---

## 🎯 Commandes Jenkins CLI (optionnel)

Si Jenkins CLI est configuré:

```powershell
# Lancer un build
java -jar jenkins-cli.jar -s http://localhost:8080/ build Order.API-Pipeline

# Lancer avec paramètres
java -jar jenkins-cli.jar -s http://localhost:8080/ build Order.API-Pipeline -p SERVICE=Order.API -p DEPLOY=true

# Voir les logs
java -jar jenkins-cli.jar -s http://localhost:8080/ console Order.API-Pipeline

# Liste des jobs
java -jar jenkins-cli.jar -s http://localhost:8080/ list-jobs
```

---

**✨ Votre pipeline Jenkins pour Order.API est configuré et prêt à l'emploi !**

Pour toute question, consultez les logs Jenkins ou les fichiers de documentation associés.
