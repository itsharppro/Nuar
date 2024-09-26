#!/bin/bash

base_dir="src"
divider="----------------------------------------"

# Check if [pack-all-force] flag is present in the latest commit message
FORCE_PACK_ALL=$(git log -1 --pretty=%B | grep -q "\[pack-all-force\]" && echo true || echo false)

# Determine changed files based on branch or commit difference
if [[ "$GITHUB_EVENT_NAME" == "pull_request" && "$GITHUB_BASE_REF" == "main" ]]; then
  CHANGED_FILES=$(git diff --name-only origin/main origin/dev)
else
  CHANGED_FILES=$(git diff --name-only $GITHUB_SHA~1 $GITHUB_SHA)
fi

echo "$divider"
echo "Starting packaging process"
echo "$divider"

# Function to check if a directory has changes
directory_contains_changes() {
  local dir="$1"
  [[ "$FORCE_PACK_ALL" == true ]] || [[ $(echo "$CHANGED_FILES" | grep -c "$dir") -gt 0 ]]
}

# Process each package directory
for dir in "$base_dir"/*/
do
  package_name=${dir##*/}
  script_path="$dir/scripts/build-and-pack.sh"
  
  echo "$divider"
  echo "Checking package: $package_name"
  echo "$divider"
  
  if directory_contains_changes "$dir"; then
    if [ -f "$script_path" ]; then
      echo "Executing packaging script for: $package_name"
      chmod +x "$script_path"
      "$script_path" || { echo "Packaging failed for $package_name"; exit 1; }
    else
      echo "No packaging script found for $package_name"
    fi
  else
    echo "Skipping $package_name (no changes detected)"
  fi
done

echo "$divider"
echo "Packaging process finished"
echo "$divider"
