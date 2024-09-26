#!/bin/bash

FORCE_BUILD_TEST=$(git log -1 --pretty=%B | grep -q "\[build-test-force\]" && echo true || echo false)

CHANGED_FILES=$(git diff --name-only ${GITHUB_EVENT_BEFORE:-HEAD~1} ${GITHUB_SHA:-HEAD})

LIBRARIES=($(echo "$CHANGED_FILES" | grep "\.csproj" | cut -d'/' -f3 | sort -u))

if [ ${#LIBRARIES[@]} -eq 0 ] || [ "$FORCE_BUILD_TEST" = true ]; then
  LIBRARIES=($(find src/* -maxdepth 0 -type d -exec basename {} \;))
fi

echo "${LIBRARIES[@]}" | jq -R -s -c 'split(" ")' > libraries.json
echo "Libraries to be processed: ${LIBRARIES[@]}"
