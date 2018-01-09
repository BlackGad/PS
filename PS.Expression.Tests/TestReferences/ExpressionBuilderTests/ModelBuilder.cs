using System;
using System.Collections.Generic;
using PS.Expression.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Expression.Tests.TestReferences.ExpressionBuilderTests
{
    class ModelBuilder
    {
        #region Static members

        public static List<License> CreateModel()
        {
            var template = new Template();
            return new List<License>
            {
                new License
                {
                    Id = Guid.NewGuid(),
                    Template = template,
                    Claims = new List<Claim>
                    {
                        new Claim
                        {
                            Id = Guid.NewGuid(),
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