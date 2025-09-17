# ST10384345_PROG6212_POE1
Contract Monthly Claim System (CMCS)
A comprehensive web application for managing monthly contract lecturer claims, built with ASP.NET Core MVC.
Overview
The Contract Monthly Claim System (CMCS) is designed to streamline the process of submitting, reviewing, and processing monthly teaching claims for contract lecturers. The system provides role-based access control and a complete workflow from claim submission to payment processing.
Features
For Lecturers

Dashboard Overview: View pending claims, approved claims, total earnings, and monthly hours
Claim Management: Submit new monthly claims with hours worked and hourly rates
Document Upload: Upload supporting documents (timesheets, contracts, invoices, receipts)
Claim Tracking: Monitor claim status throughout the approval process
Document Management: View, download, and manage uploaded documents

For Programme Coordinators

Claim Review Dashboard: Review and approve/reject submitted claims
Document Verification: Access and verify supporting documents
Approval Workflow: Approve or reject claims with feedback
Statistics Overview: Track pending reviews, approvals, and rejections

For HR Staff

Payment Processing: Process approved claims for payment
Bulk Operations: Process multiple payments simultaneously
Payment Tracking: Monitor paid claims and monthly totals
Reporting: Export payment reports and analytics

System-wide Features

Role-based Access Control: Different dashboards and permissions for each user type
Responsive Design: Works on desktop, tablet, and mobile devices
Document Management: Secure file upload and storage system
Audit Trail: Track claim status changes and approval history
Search and Filtering: Find claims by status, date, lecturer, or reference number

Technology Stack

Backend: ASP.NET Core 6.0+ MVC
Frontend: Bootstrap 5.1.3, Font Awesome 6.0
Authentication: ASP.NET Core Identity
Database: Entity Framework Core (SQL Server)
File Storage: Server-side file management
UI Framework: Responsive web design with modern CSS

Prerequisites

.NET 6.0 SDK or later
SQL Server (LocalDB or full instance)
Visual Studio 2022 or VS Code
IIS Express or Kestrel server
