#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# API endpoints
CMD_API="http://localhost:5262"
QUERY_API="http://localhost:5263"

echo "=========================================="
echo "  CQRS Event Sourcing Service Health Check"
echo "=========================================="
echo ""

# Function to check if service is running
check_service() {
    local name=$1
    local url=$2

    echo -n "Checking $name... "

    response=$(curl -s -o /dev/null -w "%{http_code}" "$url" 2>/dev/null)

    if [ "$response" == "200" ] || [ "$response" == "404" ]; then
        echo -e "${GREEN}✓ Running${NC}"
        return 0
    else
        echo -e "${RED}✗ Not responding (HTTP: $response)${NC}"
        return 1
    fi
}

# Function to test creating a post
test_create_post() {
    echo -n "Testing POST creation... "

    local test_message="Health check test post at $(date '+%Y-%m-%d %H:%M:%S')"

    response=$(curl -s -w "\n%{http_code}" -X POST "$CMD_API/api/v1/posts" \
        -H "Content-Type: application/json" \
        -d "{\"author\":\"HealthCheck\",\"message\":\"$test_message\"}")

    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')

    if [ "$http_code" == "201" ]; then
        POST_ID=$(echo "$body" | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
        echo -e "${GREEN}✓ Created (ID: $POST_ID)${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to test querying posts
test_query_posts() {
    echo -n "Testing POST query... "

    response=$(curl -s -w "\n%{http_code}" -X GET "$QUERY_API/api/v1/posts")

    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')

    if [ "$http_code" == "200" ]; then
        post_count=$(echo "$body" | grep -o '"postId"' | wc -l)
        echo -e "${GREEN}✓ Success (Found $post_count posts)${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to test querying specific post
test_query_post_by_id() {
    if [ -z "$POST_ID" ]; then
        echo -e "${YELLOW}⊘ Skipping query by ID (no post created)${NC}"
        return 0
    fi

    echo -n "Testing query by ID... "

    # Wait a bit for event processing
    sleep 2

    response=$(curl -s -w "\n%{http_code}" -X GET "$QUERY_API/api/v1/posts/$POST_ID")

    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')

    if [ "$http_code" == "200" ]; then
        echo -e "${GREEN}✓ Found post${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to test editing a post
test_edit_post() {
    if [ -z "$POST_ID" ]; then
        echo -e "${YELLOW}⊘ Skipping edit test (no post created)${NC}"
        return 0
    fi

    echo -n "Testing POST edit... "

    local updated_message="Updated health check message at $(date '+%Y-%m-%d %H:%M:%S')"

    response=$(curl -s -w "\n%{http_code}" -X PUT "$CMD_API/api/v1/posts/$POST_ID/message" \
        -H "Content-Type: application/json" \
        -d "{\"message\":\"$updated_message\"}")

    http_code=$(echo "$response" | tail -n1)

    if [ "$http_code" == "200" ]; then
        echo -e "${GREEN}✓ Updated${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to test liking a post
test_like_post() {
    if [ -z "$POST_ID" ]; then
        echo -e "${YELLOW}⊘ Skipping like test (no post created)${NC}"
        return 0
    fi

    echo -n "Testing POST like... "

    response=$(curl -s -w "\n%{http_code}" -X PUT "$CMD_API/api/v1/posts/$POST_ID/like")

    http_code=$(echo "$response" | tail -n1)

    if [ "$http_code" == "200" ]; then
        echo -e "${GREEN}✓ Liked${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to test adding comment
test_add_comment() {
    if [ -z "$POST_ID" ]; then
        echo -e "${YELLOW}⊘ Skipping comment test (no post created)${NC}"
        return 0
    fi

    echo -n "Testing comment add... "

    response=$(curl -s -w "\n%{http_code}" -X POST "$CMD_API/api/v1/posts/$POST_ID/comments" \
        -H "Content-Type: application/json" \
        -d "{\"comment\":\"Health check comment\",\"username\":\"HealthCheckBot\"}")

    http_code=$(echo "$response" | tail -n1)

    if [ "$http_code" == "200" ]; then
        echo -e "${GREEN}✓ Added${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to test deleting a post
test_delete_post() {
    if [ -z "$POST_ID" ]; then
        echo -e "${YELLOW}⊘ Skipping delete test (no post created)${NC}"
        return 0
    fi

    echo -n "Testing POST delete... "

    response=$(curl -s -w "\n%{http_code}" -X DELETE "$CMD_API/api/v1/posts/$POST_ID" \
        -H "Content-Type: application/json" \
        -d "{\"username\":\"HealthCheck\"}")

    http_code=$(echo "$response" | tail -n1)

    if [ "$http_code" == "200" ]; then
        echo -e "${GREEN}✓ Deleted${NC}"
        return 0
    else
        echo -e "${RED}✗ Failed (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Function to verify deletion in query database
test_verify_deletion() {
    if [ -z "$POST_ID" ]; then
        echo -e "${YELLOW}⊘ Skipping deletion verification (no post created)${NC}"
        return 0
    fi

    echo -n "Verifying deletion in Query DB... "

    # Wait for event processing
    sleep 2

    response=$(curl -s -w "\n%{http_code}" -X GET "$QUERY_API/api/v1/posts/$POST_ID")

    http_code=$(echo "$response" | tail -n1)

    if [ "$http_code" == "204" ] || [ "$http_code" == "404" ]; then
        echo -e "${GREEN}✓ Deleted from Query DB${NC}"
        return 0
    else
        echo -e "${RED}✗ Still exists (HTTP: $http_code)${NC}"
        return 1
    fi
}

# Main test execution
echo "1. Infrastructure Health Checks"
echo "================================"
check_service "Command API" "$CMD_API/swagger/index.html"
cmd_status=$?

check_service "Query API" "$QUERY_API/swagger/index.html"
query_status=$?

echo ""
echo "2. End-to-End CQRS Flow Test"
echo "============================="

if [ $cmd_status -eq 0 ] && [ $query_status -eq 0 ]; then
    # Create a test post
    test_create_post
    create_status=$?

    # Run all other tests
    test_edit_post
    edit_status=$?

    test_like_post
    like_status=$?

    test_add_comment
    comment_status=$?

    test_query_post_by_id
    query_id_status=$?

    test_query_posts
    query_all_status=$?

    test_delete_post
    delete_status=$?

    test_verify_deletion
    verify_status=$?

    echo ""
    echo "=========================================="
    echo "  Test Summary"
    echo "=========================================="

    total_tests=8
    passed_tests=0

    [ $cmd_status -eq 0 ] && ((passed_tests++))
    [ $query_status -eq 0 ] && ((passed_tests++))
    [ $create_status -eq 0 ] && ((passed_tests++))
    [ $edit_status -eq 0 ] && ((passed_tests++))
    [ $like_status -eq 0 ] && ((passed_tests++))
    [ $comment_status -eq 0 ] && ((passed_tests++))
    [ $query_id_status -eq 0 ] && ((passed_tests++))
    [ $query_all_status -eq 0 ] && ((passed_tests++))

    if [ $passed_tests -eq $total_tests ]; then
        echo -e "${GREEN}✓ All tests passed ($passed_tests/$total_tests)${NC}"
        echo -e "${GREEN}Service is healthy!${NC}"
        exit 0
    else
        echo -e "${RED}✗ Some tests failed ($passed_tests/$total_tests passed)${NC}"
        exit 1
    fi
else
    echo -e "${RED}Cannot run end-to-end tests - services are not running${NC}"
    exit 1
fi
