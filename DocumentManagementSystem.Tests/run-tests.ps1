# Test Runner Script for Document Management System
# This script runs all unit tests and generates a comprehensive report

Write-Host "==========================================" -ForegroundColor Green
Write-Host "Document Management System - Test Runner" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""

# Navigate to the test project directory
Set-Location $PSScriptRoot

# Clean previous test results
if (Test-Path "TestResults") {
    Remove-Item "TestResults" -Recurse -Force
    Write-Host "Cleaned previous test results" -ForegroundColor Yellow
}

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore

# Build the test project
Write-Host "Building test project..." -ForegroundColor Cyan
dotnet build --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Please fix the build errors before running tests." -ForegroundColor Red
    exit 1
}

# Run tests with coverage
Write-Host "Running unit tests with coverage..." -ForegroundColor Cyan
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults --logger "console;verbosity=detailed"

# Check if tests passed
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host "All tests passed successfully!" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Red
    Write-Host "Some tests failed!" -ForegroundColor Red
    Write-Host "==========================================" -ForegroundColor Red
    exit 1
}

# Generate test summary
Write-Host ""
Write-Host "Test Summary:" -ForegroundColor Yellow
Write-Host "=============" -ForegroundColor Yellow

# Count test files
$testFiles = Get-ChildItem -Path "*.cs" -Filter "*Tests.cs" | Measure-Object
Write-Host "Test Files: $($testFiles.Count)" -ForegroundColor White

# Count test methods (approximate)
$testMethods = Get-ChildItem -Path "*.cs" -Filter "*Tests.cs" | Get-Content | Select-String "\[Fact\]" | Measure-Object
Write-Host "Test Methods: $($testMethods.Count)" -ForegroundColor White

Write-Host ""
Write-Host "Test Categories:" -ForegroundColor Yellow
Write-Host "================" -ForegroundColor Yellow
Write-Host "✓ Authentication Tests (AuthService)" -ForegroundColor Green
Write-Host "✓ Document Management Tests (DocumentService)" -ForegroundColor Green
Write-Host "✓ AI Integration Tests (GeminiService)" -ForegroundColor Green
Write-Host "✓ Controller Tests (AuthController, DocumentController)" -ForegroundColor Green
Write-Host "✓ Document Retrieval Tests" -ForegroundColor Green
Write-Host "✓ Document Authorization Tests" -ForegroundColor Green
Write-Host "✓ LLM Integration Tests" -ForegroundColor Green
Write-Host "✓ LLM Error Handling Tests" -ForegroundColor Green
Write-Host "✓ Search Functionality Tests" -ForegroundColor Green
Write-Host "✓ File Processing Tests" -ForegroundColor Green
Write-Host "✓ Error Handling Tests" -ForegroundColor Green

Write-Host ""
Write-Host "Test Coverage Areas:" -ForegroundColor Yellow
Write-Host "====================" -ForegroundColor Yellow
Write-Host "• User Registration and Authentication" -ForegroundColor White
Write-Host "• Document Upload and Processing" -ForegroundColor White
Write-Host "• Document Search and Retrieval" -ForegroundColor White
Write-Host "• Document Update and Deletion" -ForegroundColor White
Write-Host "• Document Authorization and Ownership" -ForegroundColor White
Write-Host "• Advanced Search Functionality" -ForegroundColor White
Write-Host "• Natural Language Queries" -ForegroundColor White
Write-Host "• AI-powered Summarization" -ForegroundColor White
Write-Host "• Keyword Extraction" -ForegroundColor White
Write-Host "• LLM Error Handling and Resilience" -ForegroundColor White
Write-Host "• File Type Validation" -ForegroundColor White
Write-Host "• Error Scenarios and Edge Cases" -ForegroundColor White
Write-Host "• API Failure Scenarios" -ForegroundColor White
Write-Host "• Concurrent Request Handling" -ForegroundColor White

Write-Host ""
Write-Host "Test Results Location: TestResults/" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Green 