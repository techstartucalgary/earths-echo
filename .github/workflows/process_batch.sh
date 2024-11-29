#!/bin/bash

# process_batch.sh
# Arguments:
# $1 - batch content (diffs)
# $2 - code review prompt
# $3 - batch number
# $4 - batch files (list of files in this batch)

BATCH_CONTENT="$1"
PROMPT="$2"
BATCH_NUMBER="$3"
BATCH_FILES="$4"

echo "Processing batch $BATCH_NUMBER with files: $BATCH_FILES"

# Escape newlines and double quotes in the batch content
escaped_code=$(echo -e "$BATCH_CONTENT" | jq -s -R -r @json)

# Prepare the request payload
request_payload=$(jq -n \
  --arg model "gpt-3.5-turbo" \
  --arg prompt "$PROMPT" \
  --arg code "$escaped_code" \
  '{
    "model": $model,
    "messages": [
      { "role": "system", "content": $prompt },
      { "role": "user", "content": $code }
    ]
  }')

# Send the request to OpenAI API
response=$(curl -s https://api.openai.com/v1/chat/completions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $OPEN_AI_KEY" \
  -d "$request_payload")

# Check for API errors
error_message=$(echo "$response" | jq -r '.error.message // empty')
if [ -n "$error_message" ]; then
  echo "OpenAI API Error: $error_message"
  exit 1
fi

# Extract the assistant's reply
code_review_suggestions=$(echo "$response" | jq -r '.choices[0].message.content')

# Append the suggestions to the all_code_suggestions.txt file
echo "## Review for Batch $BATCH_NUMBER" >> all_code_suggestions.txt
echo "### Files: $BATCH_FILES" >> all_code_suggestions.txt
echo "" >> all_code_suggestions.txt
echo "$code_review_suggestions" >> all_code_suggestions.txt
echo "" >> all_code_suggestions.txt

# Optional: Delay to comply with rate limits (e.g., 5 seconds)
sleep 5
