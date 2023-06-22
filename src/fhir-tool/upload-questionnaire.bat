@echo off
.\fhir-tool.exe upload --format json --questionnaire %1 --environment %2 --fhir-version R4
