﻿/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using FhirTool.Core.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace FhirTool.Core.ArgumentHelpers
{
    public static class DefinedEnvironments
    {
        public static IEnumerable<EnvironmentSettings> Environments { get; set; }

        public static EnvironmentSettings GetEnvironment(string env)
        {
            return Environments.FirstOrDefault(it => it.Name == env);
        }

        public static bool HasEnvironment(string env)
        {
            return GetEnvironment(env) != null;
        }
    }

    public class WithEnvironment
    {
        public string Environment { get; }

        public string ProxyBaseUrl { get; set; }
        public string FhirBaseUrl { get; set; }
        public string AuthorizationUrl { get; set; }

        public WithEnvironment(string environment)
        {
            Environment = environment;

            var e = DefinedEnvironments.GetEnvironment(environment);
            if (e != null)
            {
                FhirBaseUrl = e.FhirBaseUrl.AppendCharToEndOfStringIfMissing('/');
                ProxyBaseUrl = e.ProxyBaseUrl.AppendCharToEndOfStringIfMissing('/');
                AuthorizationUrl = e.AuthorizationUrl.AppendCharToEndOfStringIfMissing('/');
            }
        }

        public void Validate(string paramName)
        {
            if (Environment != null && !DefinedEnvironments.HasEnvironment(Environment))
            {
                var known = string.Join(", ", DefinedEnvironments.Environments.Select(it => it.Name));
                throw new SemanticArgumentException($"environment {Environment} is not a known environment. Known environments are {known}", paramName);
            }
        }
    }
}
