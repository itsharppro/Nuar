#!/bin/bash

base_dir="src"
divider="----------------------------------------"

COMMIT_MESSAGE=$(git log -1 --pretty=%B)

# Check if the commit message contains [pack-all-force]
if [[ "$COMMIT_MESSAGE" == *"[pack-all-force]"* ]]; then
    echo "$divider"
    echo "Commit message contains [pack-all-force]. Forcing packaging of all libraries."
    echo "$divider"
    FORCE_PACK_ALL=true
else
    FORCE_PACK_ALL=false
fi

if [[ "$GITHUB_EVENT_NAME" == "pull_request" && "$GITHUB_BASE_REF" == "main" ]]; then
    echo "$divider"
    echo "Pull request detected targeting the main branch."
    echo "Comparing changes between the 'dev' and 'main' branches..."
    echo "$divider"
    CHANGED_FILES=$(git diff --name-only origin/main origin/dev)
else
    echo "$divider"
    echo "No pull request detected or not targeting main. Checking commit differences."
    echo "$divider"
    CHANGED_FILES=$(git diff --name-only $GITHUB_SHA~1 $GITHUB_SHA)
fi

# Function to check if the directory contains changes
directory_contains_changes() {
    local dir="$1"
    if $FORCE_PACK_ALL; then
        return 0 
    fi
    for file in $CHANGED_FILES; do
        if [[ "$file" == "$dir"* ]]; then
            return 0  
        fi
    done
    return 1  # No changed files in the directory
}

echo "$divider"
echo "Starting the process of packaging libraries"
echo "$divider"

# Define the specific order for Nuar framework
declare -a packages=(
    "Nuar"                # Core Nuar
    "Nuar.JWT"            # Extensions
    "Nuar.Cors"           # Extensions
    "Nuar.Swagger"        # Extensions
    "Nuar.CustomErrors"    # Extensions
    "Nuar.RabbitMQ"       # Extensions
    "Nuar.Tracing"        # Extensions
    "Nuar.Host"           # Host should be packed last
)

# Iterate through the defined order of packages
for package in "${packages[@]}"; do
    dir="$base_dir/$package"
    script_path="$dir/scripts/build-and-pack.sh"

    echo "$divider"
    echo "Checking package: $package"
    echo "$divider"

    if directory_contains_changes "$dir"; then
        echo "Processing package: $package"
        echo "$divider"

        if [ -f "$script_path" ]; then
            echo "Found packaging script for: $package"
            echo "$divider"

            chmod +x "$script_path"

            echo "Executing packaging script for: $package"
            echo "$divider"
            "$script_path"

            if [ $? -ne 0 ]; then
                echo "$divider"
                echo "Error: Packaging failed for $package"
                echo "$divider"
                exit 1
            fi

            echo "$divider"
            echo "Successfully packed and published: $package"
            echo "$divider"
        else
            echo "$divider"
            echo "Warning: No packaging script found for $package"
            echo "$divider"
        fi
    else
        echo "$divider"
        echo "Skipping package: $package (no changes detected)"
        echo "$divider"
    fi
done

echo "$divider"
echo "Finished processing all NuGet packages."
echo "$divider"
