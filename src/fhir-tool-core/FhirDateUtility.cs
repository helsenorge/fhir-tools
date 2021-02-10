﻿/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

extern alias R3;

using R3::Hl7.Fhir.Model;
using System;

namespace FhirTool.Core
{
    internal static class DateUtility
    {
        public static bool IsValidFhirDateTime(string dateTime)
        {
            try
            {
                FhirDateTime dt = new FhirDateTime(dateTime);
                DateTimeOffset offset = dt.ToDateTimeOffset(TimeSpan.Zero);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool IsValidFhirDate(string date)
        {
            try
            {
                Date d = new Date(date);
                DateTimeOffset? offset = d.ToDateTimeOffset();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
