﻿extern alias R3;
extern alias R4;

using System;
using System.Collections.Generic;

using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Conversion.Converters.Support.TypeMaps
{
    internal static class R4ToR3TypeMappings
    {
        public static Dictionary<Type, Type> Map = 
            new Dictionary<Type, Type> {
                {typeof(R4Model.Questionnaire.ItemComponent), typeof(R3Model.Questionnaire.ItemComponent) },
                {typeof(R4Model.Questionnaire.EnableWhenComponent), typeof(R3Model.Questionnaire.EnableWhenComponent) },

                {typeof(R4Model.QuestionnaireResponse.AnswerComponent), typeof(R3Model.QuestionnaireResponse.AnswerComponent)},
                {typeof(R4Model.QuestionnaireResponse.ItemComponent), typeof(R3Model.QuestionnaireResponse.ItemComponent)},

                {typeof(R4Model.ValueSet.ComposeComponent), typeof(R3Model.ValueSet.ComposeComponent)},
                {typeof(R4Model.ValueSet.ConceptReferenceComponent), typeof(R3Model.ValueSet.ConceptReferenceComponent)},
                {typeof(R4Model.ValueSet.ConceptSetComponent), typeof(R3Model.ValueSet.ConceptSetComponent)},
                {typeof(R4Model.ValueSet.ContainsComponent), typeof(R3Model.ValueSet.ContainsComponent)},
                {typeof(R4Model.ValueSet.DesignationComponent), typeof(R3Model.ValueSet.DesignationComponent)},
                {typeof(R4Model.ValueSet.ExpansionComponent), typeof(R3Model.ValueSet.ExpansionComponent)},
                {typeof(R4Model.ValueSet.FilterComponent), typeof(R3Model.ValueSet.FilterComponent)},
                {typeof(R4Model.ValueSet.ParameterComponent), typeof(R3Model.ValueSet.ParameterComponent)},

                {typeof(R4Model.Bundle.EntryComponent), typeof(R3Model.Bundle.EntryComponent)},
                {typeof(R4Model.Bundle.LinkComponent), typeof(R3Model.Bundle.LinkComponent)},
                {typeof(R4Model.Bundle.RequestComponent), typeof(R3Model.Bundle.RequestComponent)},
                {typeof(R4Model.Bundle.ResponseComponent), typeof(R3Model.Bundle.ResponseComponent)},
                {typeof(R4Model.Bundle.SearchComponent), typeof(R3Model.Bundle.SearchComponent)},
            };
    }
}
