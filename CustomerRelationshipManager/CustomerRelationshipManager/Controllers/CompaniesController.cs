﻿using CustomerRelationshipManager.DataRepositories;
using CustomerRelationshipManager.Models;
using CustomerRelationshipManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerRelationshipManager.Controllers
{
    [Authorize]
    public class CompaniesController: Controller
    {
        private ICompanyRepository _companyRepository;
        private IDataRepository<BusinessIndustry> _businessIndustryRepository;

        public CompaniesController(ICompanyRepository companyRepository, 
            IDataRepository<BusinessIndustry> businessIndustryRepository)
        {
            _companyRepository = companyRepository;
            _businessIndustryRepository = businessIndustryRepository;
        }

        public IActionResult All()
        {
            IEnumerable<Company> companies = _companyRepository.GetAll().ToList();
            foreach(Company company in companies)
            {
                _companyRepository.FillBusinessIndustryNavProperty(company);
            }
            return View(companies);
        }

        public IActionResult Index()
        {
            return RedirectToAction("All");
        }

        [HttpGet]
        public IActionResult Create()
        {
            CreateOrEditCompanyViewModel model = new CreateOrEditCompanyViewModel()
            {
                BusinessIndustries = _businessIndustryRepository.GetAll().ToList()
            };

            byte[] userWhoAddedIDBytes;
            int userWhoAddedID;
            if (HttpContext.Session.TryGetValue("UserID", out userWhoAddedIDBytes))
            {
                userWhoAddedID = BitConverter.ToInt32(userWhoAddedIDBytes, 0);
                model.UserWhoAddedID = userWhoAddedID;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateOrEditCompanyViewModel createCompanyViewModel)
        {
            if (ModelState.IsValid)
            {
                createCompanyViewModel.DateAdded = DateTime.Now;
                _companyRepository.Add(createCompanyViewModel);
                return RedirectToAction("Details", new { ID = createCompanyViewModel.ID });
            }
            createCompanyViewModel.BusinessIndustries = _businessIndustryRepository.GetAll().ToList();
            return View(createCompanyViewModel);
        }

        public IActionResult Details(int ID)
        {
            Company company = _companyRepository.Get(ID);
            _companyRepository.FillBusinessIndustryNavProperty(company);
            _companyRepository.FillBusinessNotesNavProperty(company);
            _companyRepository.FillContactPeopleNavProperty(company);
            return View(company);
        }

        [HttpGet]
        public IActionResult Edit(int ID)
        {
            Company company = _companyRepository.Get(ID);
            CreateOrEditCompanyViewModel model = new CreateOrEditCompanyViewModel()
            {
                ID = company.ID,
                Address = company.Address,
                BusinessIndustryID = company.BusinessIndustryID,
                City = company.City,
                DateAdded = company.DateAdded,
                IsDeleted = company.IsDeleted,
                Name = company.Name,
                NIP = company.NIP,
                UserWhoAddedID = company.UserWhoAddedID,
                BusinessIndustries = _businessIndustryRepository.GetAll().ToList()

            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CreateOrEditCompanyViewModel model)
        {
            if(ModelState.IsValid)
            {
                Company company = _companyRepository.Get(model.ID);
                company.Name = model.Name;
                company.BusinessIndustryID = model.BusinessIndustryID;
                company.Address = model.Address;
                company.City = model.City;
                company.NIP = model.NIP;
                _companyRepository.Edit(company);
                return RedirectToAction("all");
            }
            return View(model);
        }
    }
    
}
