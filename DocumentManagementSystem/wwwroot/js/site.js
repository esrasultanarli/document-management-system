// Document Management System - Modern JavaScript

document.addEventListener("DOMContentLoaded", function () {
  initializeApp();
});

function initializeApp() {
  setupFileUpload();
  setupAnimations();
  setupSearchEnhancements();
  setupInteractiveElements();
  setupFormValidation();
  setupDropdownMenu();
}

// Dropdown Menu Management
function setupDropdownMenu() {
  const userMenu = document.querySelector(".modern-user-menu");
  const dropdown = document.querySelector(".modern-dropdown");

  if (userMenu && dropdown) {
    // Toggle dropdown on click
    userMenu.addEventListener("click", function (e) {
      e.preventDefault();
      e.stopPropagation();

      const isOpen = dropdown.classList.contains("show");

      // Close all other dropdowns
      document.querySelectorAll(".modern-dropdown.show").forEach((dd) => {
        dd.classList.remove("show");
      });

      // Toggle current dropdown
      if (!isOpen) {
        dropdown.classList.add("show");
        userMenu.classList.add("show");
      } else {
        dropdown.classList.remove("show");
        userMenu.classList.remove("show");
      }
    });

    // Close dropdown when clicking outside
    document.addEventListener("click", function (e) {
      if (!userMenu.contains(e.target) && !dropdown.contains(e.target)) {
        dropdown.classList.remove("show");
        userMenu.classList.remove("show");
      }
    });

    // Close dropdown when pressing Escape key
    document.addEventListener("keydown", function (e) {
      if (e.key === "Escape") {
        dropdown.classList.remove("show");
        userMenu.classList.remove("show");
      }
    });

    // Prevent dropdown from closing when clicking inside it
    dropdown.addEventListener("click", function (e) {
      e.stopPropagation();
    });
  }
}

// File Upload Enhancements
function setupFileUpload() {
  const fileInput = document.querySelector('input[type="file"]');
  const uploadArea = document.querySelector(".file-upload-area");

  if (fileInput && uploadArea) {
    // Drag and drop functionality
    uploadArea.addEventListener("dragover", function (e) {
      e.preventDefault();
      uploadArea.classList.add("dragover");
    });

    uploadArea.addEventListener("dragleave", function (e) {
      e.preventDefault();
      uploadArea.classList.remove("dragover");
    });

    uploadArea.addEventListener("drop", function (e) {
      e.preventDefault();
      uploadArea.classList.remove("dragover");
      const files = e.dataTransfer.files;
      if (files.length > 0) {
        fileInput.files = files;
        updateFileName(files[0].name);
      }
    });

    // Click to upload
    uploadArea.addEventListener("click", function () {
      fileInput.click();
    });

    // File selection
    fileInput.addEventListener("change", function () {
      if (this.files.length > 0) {
        updateFileName(this.files[0].name);
        validateFile(this.files[0]);
      }
    });
  }
}

function updateFileName(fileName) {
  const fileNameDisplay = document.querySelector(".file-name-display");
  if (fileNameDisplay) {
    fileNameDisplay.textContent = fileName;
    fileNameDisplay.style.display = "block";
  }
}

function validateFile(file) {
  const allowedTypes = [".pdf", ".txt", ".docx"];
  const fileExtension = "." + file.name.split(".").pop().toLowerCase();

  if (!allowedTypes.includes(fileExtension)) {
    showAlert(
      "Hata: Sadece PDF, TXT ve DOCX dosyaları yüklenebilir.",
      "danger"
    );
    return false;
  }

  const maxSize = 10 * 1024 * 1024; // 10MB
  if (file.size > maxSize) {
    showAlert("Hata: Dosya boyutu 10MB'dan büyük olamaz.", "danger");
    return false;
  }

  return true;
}

// Animations
function setupAnimations() {
  // Fade in elements on page load
  const elements = document.querySelectorAll(".card, .alert, .search-result");
  elements.forEach((element, index) => {
    element.style.opacity = "0";
    element.style.transform = "translateY(20px)";

    setTimeout(() => {
      element.style.transition = "all 0.5s ease";
      element.style.opacity = "1";
      element.style.transform = "translateY(0)";
    }, index * 100);
  });

  // Smooth scrolling for anchor links
  document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
    anchor.addEventListener("click", function (e) {
      e.preventDefault();
      const target = document.querySelector(this.getAttribute("href"));
      if (target) {
        target.scrollIntoView({
          behavior: "smooth",
          block: "start",
        });
      }
    });
  });
}

// Search Enhancements
function setupSearchEnhancements() {
  const searchInput = document.querySelector('input[name="SearchTerm"]');
  const searchForm = document.querySelector('form[asp-action="Search"]');

  if (searchInput) {
    // Auto-resize search input
    searchInput.addEventListener("input", function () {
      this.style.width = "auto";
      this.style.width = this.scrollWidth + 20 + "px";
    });

    // Search suggestions (placeholder)
    searchInput.addEventListener("keyup", function () {
      if (this.value.length > 2) {
        // Here you could implement search suggestions
        console.log("Searching for:", this.value);
      }
    });
  }

  if (searchForm) {
    searchForm.addEventListener("submit", function (e) {
      const searchTerm = searchInput.value.trim();
      if (searchTerm.length === 0) {
        e.preventDefault();
        showAlert("Lütfen arama terimi girin.", "warning");
      }
    });
  }
}

// Interactive Elements
function setupInteractiveElements() {
  // Card hover effects
  document.querySelectorAll(".card").forEach((card) => {
    card.addEventListener("mouseenter", function () {
      this.style.transform = "translateY(-5px) scale(1.02)";
    });

    card.addEventListener("mouseleave", function () {
      this.style.transform = "translateY(0) scale(1)";
    });
  });

  // Button click effects
  document.querySelectorAll(".btn").forEach((btn) => {
    btn.addEventListener("click", function (e) {
      // Create ripple effect
      const ripple = document.createElement("span");
      const rect = this.getBoundingClientRect();
      const size = Math.max(rect.width, rect.height);
      const x = e.clientX - rect.left - size / 2;
      const y = e.clientY - rect.top - size / 2;

      ripple.style.width = ripple.style.height = size + "px";
      ripple.style.left = x + "px";
      ripple.style.top = y + "px";
      ripple.classList.add("ripple");

      this.appendChild(ripple);

      setTimeout(() => {
        ripple.remove();
      }, 600);
    });
  });

  // Confirm delete actions
  document.querySelectorAll(".btn-danger").forEach((btn) => {
    if (
      btn.textContent.toLowerCase().includes("sil") ||
      btn.textContent.toLowerCase().includes("delete")
    ) {
      btn.addEventListener("click", function (e) {
        if (
          !confirm("Bu işlemi gerçekleştirmek istediğinizden emin misiniz?")
        ) {
          e.preventDefault();
        }
      });
    }
  });
}

// Form Validation
function setupFormValidation() {
  const forms = document.querySelectorAll("form");

  forms.forEach((form) => {
    form.addEventListener("submit", function (e) {
      const requiredFields = form.querySelectorAll("[required]");
      let isValid = true;

      requiredFields.forEach((field) => {
        if (!field.value.trim()) {
          isValid = false;
          field.classList.add("is-invalid");

          // Remove invalid class after user starts typing
          field.addEventListener("input", function () {
            this.classList.remove("is-invalid");
          });
        }
      });

      if (!isValid) {
        e.preventDefault();
        showAlert("Lütfen tüm gerekli alanları doldurun.", "warning");
      }
    });
  });
}

// Utility Functions
function showAlert(message, type = "info") {
  const alertContainer = document.createElement("div");
  alertContainer.className = `alert alert-${type} alert-dismissible fade show`;
  alertContainer.innerHTML = `
        <i class="fas fa-${getAlertIcon(type)} me-2"></i>${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

  const container =
    document.querySelector(".container-fluid") ||
    document.querySelector("main");
  if (container) {
    container.insertBefore(alertContainer, container.firstChild);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
      if (alertContainer.parentNode) {
        alertContainer.remove();
      }
    }, 5000);
  }
}

function getAlertIcon(type) {
  const icons = {
    success: "check-circle",
    danger: "exclamation-circle",
    warning: "exclamation-triangle",
    info: "info-circle",
  };
  return icons[type] || "info-circle";
}

// Loading States
function showLoading(element) {
  if (element) {
    element.disabled = true;
    const originalText = element.innerHTML;
    element.innerHTML = '<span class="loading me-2"></span>Yükleniyor...';
    element.dataset.originalText = originalText;
  }
}

function hideLoading(element) {
  if (element && element.dataset.originalText) {
    element.disabled = false;
    element.innerHTML = element.dataset.originalText;
    delete element.dataset.originalText;
  }
}

// Search Result Highlighting
function highlightSearchTerms(text, searchTerm) {
  if (!searchTerm) return text;

  const regex = new RegExp(`(${searchTerm})`, "gi");
  return text.replace(regex, "<mark>$1</mark>");
}

// Document Processing Status Updates
function updateProcessingStatus(documentId, status) {
  const statusElement = document.querySelector(
    `[data-document-id="${documentId}"] .processing-status`
  );
  if (statusElement) {
    statusElement.textContent = status;

    if (status === "Tamamlandı") {
      statusElement.className = "processing-status text-success";
    } else if (status === "İşleniyor") {
      statusElement.className = "processing-status text-warning";
      statusElement.innerHTML =
        '<i class="fas fa-spinner processing-spinner me-2"></i>İşleniyor...';
    }
  }
}

// Responsive Navigation
function setupResponsiveNav() {
  const navbarToggler = document.querySelector(".navbar-toggler");
  const navbarCollapse = document.querySelector(".navbar-collapse");

  if (navbarToggler && navbarCollapse) {
    navbarToggler.addEventListener("click", function () {
      navbarCollapse.classList.toggle("show");
    });

    // Close mobile menu when clicking outside
    document.addEventListener("click", function (e) {
      if (
        !navbarToggler.contains(e.target) &&
        !navbarCollapse.contains(e.target)
      ) {
        navbarCollapse.classList.remove("show");
      }
    });
  }
}

// Theme Toggle (if needed)
function setupThemeToggle() {
  const themeToggle = document.querySelector("#theme-toggle");
  if (themeToggle) {
    themeToggle.addEventListener("click", function () {
      document.body.classList.toggle("dark-theme");
      const isDark = document.body.classList.contains("dark-theme");
      localStorage.setItem("theme", isDark ? "dark" : "light");
    });

    // Load saved theme
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme === "dark") {
      document.body.classList.add("dark-theme");
    }
  }
}

// Export functions for global use
window.DocumentManagementSystem = {
  showAlert,
  showLoading,
  hideLoading,
  highlightSearchTerms,
  updateProcessingStatus,
};
