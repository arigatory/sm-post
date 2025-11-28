#!/bin/bash

# Deploy all Kubernetes resources for CQRS Event Sourcing App
# This script deploys all components in the correct order

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "=========================================="
echo "  Deploying CQRS App to Kubernetes"
echo "=========================================="
echo ""

# Step 1: Create namespace
echo -e "${YELLOW}Step 1: Creating namespace...${NC}"
kubectl apply -f namespace.yaml
echo -e "${GREEN}✓ Namespace created${NC}"
echo ""

# Step 2: Deploy databases
echo -e "${YELLOW}Step 2: Deploying PostgreSQL databases...${NC}"
kubectl apply -f postgres-marten.yaml
kubectl apply -f postgres-read.yaml
echo -e "${GREEN}✓ PostgreSQL deployed${NC}"
echo ""

# Step 3: Deploy Kafka
echo -e "${YELLOW}Step 3: Deploying Kafka...${NC}"
kubectl apply -f kafka.yaml
kubectl apply -f kafka-ui.yaml
echo -e "${GREEN}✓ Kafka deployed${NC}"
echo ""

# Step 4: Wait for infrastructure to be ready
echo -e "${YELLOW}Step 4: Waiting for infrastructure to be ready...${NC}"
echo "Waiting for PostgreSQL Marten..."
kubectl wait --for=condition=ready pod -l app=postgres-marten -n cqrs-app --timeout=180s

echo "Waiting for PostgreSQL Read..."
kubectl wait --for=condition=ready pod -l app=postgres-read -n cqrs-app --timeout=180s

echo "Waiting for Kafka..."
kubectl wait --for=condition=ready pod -l app=kafka -n cqrs-app --timeout=180s

echo "Waiting for Kafka UI..."
kubectl wait --for=condition=ready pod -l app=kafka-ui -n cqrs-app --timeout=180s

echo -e "${GREEN}✓ Infrastructure ready${NC}"
echo ""

# Step 5: Deploy applications
echo -e "${YELLOW}Step 5: Deploying applications (2 replicas each)...${NC}"
kubectl apply -f post-cmd-api.yaml
kubectl apply -f post-query-api.yaml
kubectl apply -f client.yaml
echo -e "${GREEN}✓ Applications deployed${NC}"
echo ""

# Step 6: Wait for applications to be ready
echo -e "${YELLOW}Step 6: Waiting for applications to be ready...${NC}"
echo "Waiting for Command API (2 replicas)..."
kubectl wait --for=condition=ready pod -l app=post-cmd-api -n cqrs-app --timeout=180s

echo "Waiting for Query API (2 replicas)..."
kubectl wait --for=condition=ready pod -l app=post-query-api -n cqrs-app --timeout=180s

echo "Waiting for Client (2 replicas)..."
kubectl wait --for=condition=ready pod -l app=client -n cqrs-app --timeout=180s

echo -e "${GREEN}✓ All applications ready${NC}"
echo ""

# Step 7: Show deployment status
echo "=========================================="
echo "  Deployment Status"
echo "=========================================="
echo ""

echo "Pods:"
kubectl get pods -n cqrs-app -o wide

echo ""
echo "Services:"
kubectl get svc -n cqrs-app

echo ""
echo "=========================================="
echo "  Access URLs"
echo "=========================================="

# Check if using minikube
if command -v minikube &> /dev/null && minikube status &> /dev/null; then
    MINIKUBE_IP=$(minikube ip)
    echo "Frontend:     http://${MINIKUBE_IP}:30000"
    echo "Command API:  http://${MINIKUBE_IP}:30262"
    echo "Query API:    http://${MINIKUBE_IP}:30263"
    echo "Kafka UI:     http://${MINIKUBE_IP}:30080"
else
    echo "Frontend:     http://localhost:30000"
    echo "Command API:  http://localhost:30262"
    echo "Query API:    http://localhost:30263"
    echo "Kafka UI:     http://localhost:30080"
fi

echo ""
echo -e "${GREEN}✓ Deployment complete!${NC}"
echo ""
echo "To view logs:"
echo "  kubectl logs -n cqrs-app -l app=post-cmd-api --tail=50 -f"
echo "  kubectl logs -n cqrs-app -l app=post-query-api --tail=50 -f"
echo ""
echo "To scale deployments:"
echo "  kubectl scale deployment post-cmd-api -n cqrs-app --replicas=3"
echo "  kubectl scale deployment post-query-api -n cqrs-app --replicas=3"
