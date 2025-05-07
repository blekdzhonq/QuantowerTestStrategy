# Tick Bar Mismatch Repro

This project contains a minimal Quantower strategy that prints the time, open, high, low, and close of the last closed bar.

The purpose is to compare the printed bar values to what's shown on the chart.

## How it works

- The strategy prints each bar's OHLC and timestamp when it closes.
- The data loading start point is configurable in the code.
- You can compare the printed output with chart bars visually.

## What to check

- For **time-based bars** (e.g. 1-minute), the printed values always match the chart.
- For **tick-based bars** (tick, range), the printed values **do not match** the chart.
- This mismatch happens regardless of the selected start point.

This repo is meant to demonstrate that issue.
