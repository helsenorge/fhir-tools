﻿# Description of the three input parameters:
# 1. (%1) The file format you want your output file in, Json or Xml
# 2. (%3) The file name of the input file (in R4 format)
# 3. (%2) The file name of output file

.\fhir-tool convert --from R4 --to R3 --format %1 --out %3 %2
