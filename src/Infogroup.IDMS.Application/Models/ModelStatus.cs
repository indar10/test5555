using Abp.UI;
using Infogroup.IDMS.Models.Dtos;
using Infogroup.IDMS.ModelStatuss;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Infogroup.IDMS.ModelQueues;

namespace Infogroup.IDMS.Models
{
    public partial class ModelsAppService : IDMSAppServiceBase, IModelsAppService
    {
        #region Model Actions
        public void ModelsActions(int modelDetailID, bool isSampleScore)
        {
            try
            {
                var result = new ModelsActionOutputDto { Success = true };
                
                var changeStatusValidation = ValidateChangeStatus(modelDetailID, isSampleScore);
                var modelDetails = _modelDetailsRepository.GetAll().FirstOrDefault(x => x.Id == modelDetailID );
                
                if (!changeStatusValidation.Success) throw new UserFriendlyException(changeStatusValidation.Message);

                var modelQueue = new ModelQueue();
                modelQueue.ModelDetailID = modelDetailID;
                modelQueue.iIsCurrent = true;
                modelQueue.iPriority = 1;
                modelQueue.cNotes = string.Empty;
                if (isSampleScore)
                {
                    modelQueue.iIsSampleScore = true;
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.SampleSubmitted).ToString();
                }
                else
                {
                    modelQueue.iIsSampleScore = false;
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.DatabaseSubmitted).ToString();
                }

                modelQueue.dScheduledDate = DateTime.Now;
                modelQueue.cCreatedBy = _mySession.IDMSUserName;
                modelQueue.dCreatedDate = DateTime.Now;

                _modelQueueRepository.Insert(modelQueue);                

                modelDetails.cModifiedBy = _mySession.IDMSUserName;
                modelDetails.dModifiedDate = DateTime.Now;

                _modelDetailsRepository.Update(modelDetails);

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private ModelsActionOutputDto ValidateChangeStatus(int modelDetailID, bool isSampleScore)
        {
            var result = new ModelsActionOutputDto { Success = true };
            var currentStatusObject = _modelQueueRepository.FirstOrDefault(o => o.ModelDetailID == modelDetailID && o.iIsCurrent);
            if(isSampleScore)
            {
                if (!currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.ModelCreated).ToString()) && !currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.SampleCompleted).ToString()) && !currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.SampleFailed).ToString()))
                {
                    result.Success = false;
                    result.Message = L("SampleScore", modelDetailID);
                }
            }
            else
            {
                if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.DatabaseSubmitted).ToString()) || currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.DatabaseRunning).ToString()))
                {
                    result.Success = false;
                    result.Message = L("DatabaseScore", modelDetailID);
                }
            }
            
            return result;
        }
        #endregion

        #region Model Action Cancel
        public void ModelsActionsCancel(int modelDetailID)
        {
            try
            {
                var currentStatusObject = _modelQueueRepository.FirstOrDefault(o => o.ModelDetailID == modelDetailID && o.iIsCurrent);

                var modelQueue = new ModelQueue();
                modelQueue.ModelDetailID = modelDetailID;
                modelQueue.iIsCurrent = true;
                modelQueue.iPriority = 1;
                modelQueue.cNotes = string.Empty;

                if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.SampleRunning).ToString()) || currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.DatabaseRunning).ToString()) || currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.ModelCreated).ToString()))
                {
                    throw new UserFriendlyException(L("CancelValidation"));
                }
                else if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.SampleSubmitted).ToString()))
                {
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.ModelCreated).ToString();
                }
                else if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.SampleFailed).ToString()))
                {
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.SampleSubmitted).ToString();
                }
                else if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.SampleCompleted).ToString()))
                {
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.SampleSubmitted).ToString();
                }
                else if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.DatabaseSubmitted).ToString()))
                {
                    var checkifSampleScored = _modelQueueRepository.Count(o => o.ModelDetailID == modelDetailID && o.LK_ModelStatus == Convert.ToInt32(ModelStatus.SampleCompleted).ToString());
                    if (checkifSampleScored > 0)
                        modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.SampleCompleted).ToString();
                    else
                        modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.ModelCreated).ToString();
                }
                else if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.DatabaseFailed).ToString()))
                {
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.DatabaseSubmitted).ToString();
                }
                else if (currentStatusObject.LK_ModelStatus.Equals(Convert.ToInt32(ModelStatus.DatabaseCompleted).ToString()))
                {
                    modelQueue.LK_ModelStatus = Convert.ToInt32(ModelStatus.DatabaseSubmitted).ToString();
                }
                modelQueue.iIsSampleScore = currentStatusObject.iIsSampleScore;
                if (modelQueue.LK_ModelStatus == "10")
                    modelQueue.dScheduledDate = null;
                else
                    modelQueue.dScheduledDate = DateTime.Now;

                modelQueue.cModifiedBy = _mySession.IDMSUserName;
                modelQueue.dModifiedDate = DateTime.Now;
                modelQueue.cCreatedBy = _mySession.IDMSUserName;
                modelQueue.dCreatedDate = DateTime.Now;

                _modelQueueRepository.Insert(modelQueue);

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Fetch Model Status
        public List<ModelStatusDto> GetStatusHistory(int modelDetailId)
        {
            try
            {
                return _customModelRepository.GetModelStatusForModelDetail(modelDetailId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        #endregion
    }
}
