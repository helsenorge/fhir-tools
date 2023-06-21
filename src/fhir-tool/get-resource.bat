@echo off
if "%4"=="" (
    .\fhir-tool get --format json --resource-type %1 --resource %2 --environment %3 --fhir-version R4
) else (
    .\fhir-tool get --format json --resource-type %1 --resource %1 --environment %3 --resource-version %4 --fhir-version R4
)
