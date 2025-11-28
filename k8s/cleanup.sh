#!/bin/bash

# Cleanup all Kubernetes resources for CQRS Event Sourcing App

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo "=========================================="
echo "  Cleanup CQRS App from Kubernetes"
echo "=========================================="
echo ""

read -p "Are you sure you want to delete all resources? (yes/no): " -r
if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
    echo "Cleanup cancelled."
    exit 0
fi

echo ""
echo -e "${YELLOW}Deleting applications...${NC}"
kubectl delete -f client.yaml --ignore-not-found=true
kubectl delete -f post-query-api.yaml --ignore-not-found=true
kubectl delete -f post-cmd-api.yaml --ignore-not-found=true
echo -e "${GREEN}✓ Applications deleted${NC}"
echo ""

echo -e "${YELLOW}Deleting Kafka...${NC}"
kubectl delete -f kafka-ui.yaml --ignore-not-found=true
kubectl delete -f kafka.yaml --ignore-not-found=true
echo -e "${GREEN}✓ Kafka deleted${NC}"
echo ""

echo -e "${YELLOW}Deleting databases...${NC}"
kubectl delete -f postgres-read.yaml --ignore-not-found=true
kubectl delete -f postgres-marten.yaml --ignore-not-found=true
echo -e "${GREEN}✓ Databases deleted${NC}"
echo ""

read -p "Delete namespace and all persistent data? (yes/no): " -r
if [[ $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
    echo ""
    echo -e "${RED}Deleting namespace (this will remove all PVCs and data)...${NC}"
    kubectl delete -f namespace.yaml --ignore-not-found=true
    echo -e "${GREEN}✓ Namespace deleted${NC}"
else
    echo ""
    echo -e "${YELLOW}Namespace preserved with persistent data${NC}"
fi

echo ""
echo -e "${GREEN}✓ Cleanup complete!${NC}"
