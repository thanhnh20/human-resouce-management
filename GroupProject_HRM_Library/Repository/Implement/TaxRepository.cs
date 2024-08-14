using AutoMapper;
using GroupProject_HRM_Library.DTOs.Tax;
using GroupProject_HRM_Library.Enums;
using GroupProject_HRM_Library.Errors;
using GroupProject_HRM_Library.Exceptions;
using GroupProject_HRM_Library.Infrastructure;
using GroupProject_HRM_Library.Models;
using GroupProject_HRM_Library.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject_HRM_Library.Repository.Implement
{
    public class TaxRepository : ITaxRepository
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaxRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateTaxRequestAsync(CreateTaxRequest request)
        {
            try
            {
                var tax = _mapper.Map<Tax>(request);
                tax.Timestamp = DateTime.Now;
                tax.TaxStatus = (int)TaxEnum.TaxStatus.InUse;

                await _unitOfWork.TaxDAO.AddNewTaxAsync(tax);
                await _unitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                List<ErrorDetail> errors = new List<ErrorDetail>();

                ErrorDetail error = new ErrorDetail()
                {
                    FieldNameError = "Exception",
                    DescriptionError = new List<string>() { ex.Message }
                };

                errors.Add(error);
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteTaxRequestAsync(int id)
        {
            try
            {
                var taxRequest = await _unitOfWork.TaxDAO.GetTaxByIDAsync(id);

                if (taxRequest == null)
                    throw new NotFoundException("The Tax with inputted ID does not exist in the System.");

                _unitOfWork.TaxDAO.DeleteTax(id);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                List<ErrorDetail> errors = new List<ErrorDetail>();

                ErrorDetail error = new ErrorDetail()
                {
                    FieldNameError = "Exception",
                    DescriptionError = new List<string>() { ex.Message }
                };

                errors.Add(error);
                if (ex.Message.Contains("The Tax with inputted ID does not exist in the System."))
                {
                    throw new NotFoundException(JsonConvert.SerializeObject(errors));
                }
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetTaxResponse> GetTaxResponseAsync(int id)
        {
            try
            {
                var taxRequest = await _unitOfWork.TaxDAO.GetTaxByIDAsync(id);

                if (taxRequest == null)
                    throw new NotFoundException("The Tax with inputted ID does not exist in the System.");

                return _mapper.Map<GetTaxResponse>(taxRequest);

            }
            catch (Exception ex)
            {
                List<ErrorDetail> errors = new List<ErrorDetail>();

                ErrorDetail error = new ErrorDetail()
                {
                    FieldNameError = "Exception",
                    DescriptionError = new List<string>() { ex.Message }
                };

                errors.Add(error);
                if (ex.Message.Contains("The Tax with inputted ID does not exist in the System."))
                {
                    throw new NotFoundException(JsonConvert.SerializeObject(errors));
                }
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetTaxResponse>> GetTaxResponsesAsync()
        {
            try
            {
                List<Tax> taxes = await _unitOfWork.TaxDAO.GetTaxesAsync();
                return _mapper.Map<List<GetTaxResponse>>(taxes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetTaxResponse>> GetTaxResponsesSortedAsync(
            decimal? minSalary, 
            decimal? maxSalary, 
            double? minPercent = null, 
            double? maxPercent = null, 
            DateTime? addDate = null,
            TaxEnum.TaxStatus? status = null,
            TaxEnum.TaxOrderBy? orderBy = null)
        {
            try
            {
                int? statusInt = null;
                if (minSalary > maxSalary)
                {
                    var t = maxSalary.Value;
                    maxSalary = minSalary;
                    minSalary = t;
                }
                if(minPercent > maxPercent)
                {
                    var t = maxPercent.Value;
                    maxPercent = minPercent;
                    minPercent = t;
                }
                if(status != null)
                {
                    statusInt = (int)status;
                }
                string? orderByString = null;
                if(orderBy != null)
                {
                    orderByString = Enum.GetName((TaxEnum.TaxOrderBy)orderBy);
                }
                List<Tax> taxes = await _unitOfWork.TaxDAO.GetTaxesSortedAsync(
                    minSalary,
                    maxSalary,
                    minPercent,
                    maxPercent,
                    addDate,
                    statusInt,
                    orderByString);
                return _mapper.Map<List<GetTaxResponse>>(taxes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateTaxRequestAsync(int id, UpdateTaxRequest request)
        {
            try
            {
                var taxRequest = await _unitOfWork.TaxDAO.GetTaxByIDAsync(id);
                if (taxRequest == null)
                    throw new NotFoundException("The Tax does not exist in the System.");

                taxRequest.SalaryMin = request.SalaryMin;
                taxRequest.SalaryMax = request.SalaryMax;
                taxRequest.Percent = request.Percent;

                _unitOfWork.TaxDAO.UpdateTax(taxRequest);
                await _unitOfWork.CommitAsync();

            }
            catch (Exception ex)
            {
                List<ErrorDetail> errors = new List<ErrorDetail>();

                ErrorDetail error = new ErrorDetail()
                {
                    FieldNameError = "Exception",
                    DescriptionError = new List<string>() { ex.Message }
                };

                errors.Add(error);
                if (ex.Message.Contains("The Tax does not exist in the System."))
                {
                    throw new NotFoundException(JsonConvert.SerializeObject(errors));
                }
                throw new Exception(ex.Message);
            }
        }
        public async Task UpdateTaxStatusRequestAsync(int id, TaxEnum.TaxStatus status)
        {
            try
            {
                var taxRequest = await _unitOfWork.TaxDAO.GetTaxByIDAsync(id);
                if (taxRequest == null)
                    throw new NotFoundException("The Tax does not exist in the System.");

                taxRequest.TaxStatus = (int)status;

                _unitOfWork.TaxDAO.UpdateTax(taxRequest);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                List<ErrorDetail> errors = new List<ErrorDetail>();

                ErrorDetail error = new ErrorDetail()
                {
                    FieldNameError = "Exception",
                    DescriptionError = new List<string>() { ex.Message }
                };

                errors.Add(error);
                if (ex.Message.Contains("The Tax does not exist in the System."))
                {
                    throw new NotFoundException(JsonConvert.SerializeObject(errors));
                }
                throw new Exception(ex.Message);
            }
        }
    }
}
