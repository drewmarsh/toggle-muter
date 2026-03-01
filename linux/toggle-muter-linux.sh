#!/bin/bash

# Dependencies: sudo apt install xdotool pulseaudio-utils libnotify-bin

# Customize Configuration
FAILURE_NOTIFICATIONS=false  # Notify if a toggle muter attempt was unsuccessful
SUCCESS_NOTIFICATIONS=false  # Notify if a toggle mute was successful

# Get the active window ID
ACTIVE_WINDOW_ID=$(xdotool getactivewindow 2>/dev/null)
if [ -z "$ACTIVE_WINDOW_ID" ]; then
    [ "$FAILURE_NOTIFICATIONS" = true ] && notify-send "Toggle Muter" "Could not get active window"
    exit 1
fi

# Get the window PID
WINDOW_PID=$(xdotool getwindowpid "$ACTIVE_WINDOW_ID" 2>/dev/null)
if [ -z "$WINDOW_PID" ]; then
    [ "$FAILURE_NOTIFICATIONS" = true ] && notify-send "Toggle Muter" "Could not get window PID"
    exit 1
fi

# Get application name for notification
APP_NAME=$(ps -p "$WINDOW_PID" -o comm= 2>/dev/null | head -1)
DISPLAY_NAME=$(xdotool getwindowname "$ACTIVE_WINDOW_ID" 2>/dev/null | head -c 50)

# Find sink inputs by PID
SINK_INPUTS=$(pactl list sink-inputs short | while read line; do
    SINK_ID=$(echo "$line" | awk '{print $1}')
    if pactl list sink-inputs | grep -A 30 "Sink Input #$SINK_ID" | grep -q "application.process.id = \"$WINDOW_PID\""; then
        echo "$SINK_ID"
    fi
done)

# If no sink inputs found by PID, try by application name
if [ -z "$SINK_INPUTS" ] && [ -n "$APP_NAME" ]; then
    SINK_INPUTS=$(pactl list sink-inputs short | while read line; do
        SINK_ID=$(echo "$line" | awk '{print $1}')
        if pactl list sink-inputs | grep -A 30 "Sink Input #$SINK_ID" | grep -q "application.name = \"$APP_NAME\""; then
            echo "$SINK_ID"
        fi
    done)
fi

if [ -z "$SINK_INPUTS" ]; then
    [ "$FAILURE_NOTIFICATIONS" = true ] && notify-send "Toggle Muter" "No audio streams found for: ${APP_NAME:-Unknown} ($DISPLAY_NAME)"
    exit 1
fi

# Toggle mute for each found sink input
MUTE_ACTION=""
for SINK_INPUT in $SINK_INPUTS; do
    CURRENT_STATE=$(pactl list sink-inputs | grep -A 10 "Sink Input #$SINK_INPUT" | grep "Mute:" | awk '{print $2}')
    if [ "$CURRENT_STATE" = "yes" ]; then
        pactl set-sink-input-mute "$SINK_INPUT" 0
        MUTE_ACTION="Unmuted"
    else
        pactl set-sink-input-mute "$SINK_INPUT" 1
        MUTE_ACTION="Muted"
    fi
done

[ "$SUCCESS_NOTIFICATIONS" = true ] && notify-send "Toggle Muter" "$MUTE_ACTION: $DISPLAY_NAME"