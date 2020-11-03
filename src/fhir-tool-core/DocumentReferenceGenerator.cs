extern alias R4;
using FhirTool.Core.Operations;
using R4::Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FhirTool.Core
{
    public class DocumentReferenceGenerator
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        public DocumentReferenceGenerator(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<DocumentReferenceGenerator>();
        }

        public DocumentReference GenerateDocumentReference(GenerateDocumentReferenceOptions arguments)
        {
            _logger.LogInformation($"Loading documentreference id: '{arguments.Id}'.");           

            return new DocumentReference
            {
                Id = arguments?.Id,
                Status = DocumentReferenceStatus.Current,
                Category = SetCategory(),
                Content = SetContent(arguments),
            };
        }
        private List<DocumentReference.ContentComponent> SetContent(GenerateDocumentReferenceOptions arguments)
        {
                var contentComponent = new DocumentReference.ContentComponent();
                var attachment = new Attachment();

                attachment.ContentType = arguments.ContentType;
                attachment.Url = $"Binary/{arguments.Id}";

                contentComponent.Attachment = attachment;

                return new List<DocumentReference.ContentComponent> { contentComponent };    
        }    

        private List<CodeableConcept> SetCategory()
        {
            var category = new List<CodeableConcept>();
            var codeableConcept = new CodeableConcept();
            var code = new Coding
            {
                System = "http://loinc.org",
                Code = "74468-0",
                Display = "Questionnaire form definition Document"
            };

            codeableConcept.Coding = new List<Coding>() { code };
            codeableConcept.Text = "Questionnaire form definition Document";
            category.Add(codeableConcept);            

            return category;
        }
    } 
}
