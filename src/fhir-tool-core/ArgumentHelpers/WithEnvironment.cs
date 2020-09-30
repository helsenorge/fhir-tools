using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FhirTool.Core.ArgumentHelpers
{
    public static class DefinedEnvironments
    {
        public static IEnumerable<FhirEnvironment> Environments { get; set; }

        public static FhirEnvironment GetEnvironment(string env)
        {
            return Environments.FirstOrDefault(it => it.Name == env);
        }

        public static bool HasEnvironment(string env)
        {
            return GetEnvironment(env) != null;
        }
    }
    public class FhirEnvironment
    {
        public string Name { get; set; }
        public string FhirBaseUrl { get; set; }
        public string ProxyBaseUrl { get; set; }
    }

    public class WithEnvironment
    {
        public string Environment { get; }

        public string ProxyBaseUrl { get; set; }
        public string FhirBaseUrl { get; set; }

        public WithEnvironment(string environment)
        {
            Environment = environment;

            FhirEnvironment e = DefinedEnvironments.GetEnvironment(environment);
            if (e != null)
            {
                FhirBaseUrl = e.FhirBaseUrl;
                ProxyBaseUrl = e.ProxyBaseUrl;
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
