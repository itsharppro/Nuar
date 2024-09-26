#!/bin/bash

# Check if [build-test-force] flag is present in the latest commit message
FORCE_BUILD_TEST=$(git log -1 --pretty=%B | grep -q "\[build-test-force\]" && echo true || echo false)

# Get libraries from libraries.json
LIBRARIES=$(jq -r '.[]' libraries.json)

for LIBRARY in $LIBRARIES; do
  echo "Processing $LIBRARY"
  
  LIBRARY_PATH="src/$LIBRARY/src/$LIBRARY"
  TEST_PATH="src/$LIBRARY/tests/$LIBRARY.Tests"
  
  if [ -d "$LIBRARY_PATH" ]; then
    if [ "$FORCE_BUILD_TEST" = true ] || git diff --name-only HEAD~1 HEAD | grep -q "$LIBRARY_PATH"; then
      echo "Restoring, building, and testing $LIBRARY"
      
      dotnet restore "$LIBRARY_PATH"
      dotnet build "$LIBRARY_PATH" --configuration Release --no-restore

      if [ -d "$TEST_PATH" ]; then
        dotnet test "$TEST_PATH" --configuration Release --collect:"XPlat Code Coverage" \
          --results-directory "$LIBRARY_PATH/TestResults/" \
          /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura \
          /p:CoverletOutput="$LIBRARY_PATH/TestResults/coverage.cobertura.xml" --logger "trx;LogFileName=TestResults.trx"
        
        COVERAGE_REPORT=$(find "$LIBRARY_PATH/TestResults/" -name "coverage.cobertura.xml" | head -n 1)
        [ -f "$COVERAGE_REPORT" ] && bash <(curl -s https://codecov.io/bash) -t "$CODECOV_TOKEN" -f "$COVERAGE_REPORT" -F $LIBRARY
      fi
    else
      echo "No changes detected for $LIBRARY"
    fi
  else
    echo "Library $LIBRARY not found."
  fi
done
