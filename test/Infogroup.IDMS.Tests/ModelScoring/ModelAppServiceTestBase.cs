using Infogroup.IDMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Tests.ModelScoring
{
    public abstract class ModelAppServiceTestBase : AppTestBase
    {
        protected readonly IModelsAppService ModelAppService;

        protected ModelAppServiceTestBase()
        {
            ModelAppService = Resolve<IModelsAppService>();
            CreateTestModels();
        }

        protected void CreateTestModels()
        {
            //Note: There is a default "admin" user also

            UsingDbContext(
              context =>
              {
                  context.Models.Add(CreateModelEntity(955, "Model1", 1, "Description1", "400", "ABC", false, 50, true));
                  context.Models.Add(CreateModelEntity(955, "Model2", 2, "Description2", "200", "DEF", false, 51, true));
                  context.Models.Add(CreateModelEntity(955, "Model3", 3, "Description3", "100", "GHI", false, 52, true));
              });
        }

        protected Model CreateModelEntity(int databaseId, string cModelName, int iIntercept, string cDescription, string cModelNumber, string cClientName, bool iIsScoredForEveryBuild, int nChildTableNumber, bool iIsActive)
        {
            var model = new Model
            {
                DatabaseID = databaseId,
                cModelName = cModelName,
                iIntercept = iIntercept,
                cDescription = cDescription,
                cModelNumber = cModelNumber,
                cClientName = cClientName,
                iIsScoredForEveryBuild = iIsScoredForEveryBuild,
                nChildTableNumber = nChildTableNumber,
                iIsActive = iIsActive,
                cCreatedBy = "Test"
            };


            return model;
        }
    }
}
