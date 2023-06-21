@echo off
.\fhir-tool.exe generate --excel-sheet-version 2 --format json --questionnaire %1 --valueset %2
