using System;
using System.Collections.Generic;
using PS.Query.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Query.Tests.TestReferences.ExpressionBuilderTests
{
    public class ModelBuilder
    {
        #region Static members

        public static List<License> CreateModel()
        {
            var template = new Template
            {
                Name = "1"
            };

            return new List<License>
            {
                new License
                {
                    Id = new Guid("dcec5d8f-6c10-4c0c-9e1b-1c9abc2f268c"),
                    Template = template,
                    Claims = new List<Claim>
                    {
                        new Claim
                        {
                            Id = new Guid("373474BF-8242-4BE8-88FA-53FFDC3BA20D"),
                            Type = "claim_type_1",
                            Name = "Claim Type 1 Value 1"
                        },
                        new Claim
                        {
                            Id = Guid.NewGuid(),
                            Type = "claim_type_1",
                            Name = "Claim Type 1 Value 1"
                        },
                        new Claim
                        {
                            Id = Guid.NewGuid(),
                            Type = "claim_type_2",
                            Name = "Claim Type 2 Value 1"
                        },
                        new Claim
                        {
                            Id = Guid.NewGuid(),
                            Type = "claim_type_1",
                            Name = "Claim Type 2 Value 2"
                        }
                    }
                },
                new License
                {
                    Id = Guid.NewGuid(),
                    Template = template,
                    Claims = new List<Claim>
                    {
                        new Claim
                        {
                            Id = Guid.NewGuid(),
                            Type = "claim_type_3",
                            Name = "Claim Type 3 Value 1"
                        }
                    }
                }
            };
        }

        #endregion
    }
}