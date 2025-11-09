// Solution robuste pour les onglets Bootstrap avec Blazor
window.blazorTabs = {
    initialized: false,
    
    // Initialiser les onglets
    init: function() {
        console.log('Initialisation des onglets Blazor...');
        
 // Nettoyer les anciens événements
        this.cleanup();
        
        // Attendre que Bootstrap soit disponible
        this.waitForBootstrap(() => {
   this.setupTabs();
   this.initialized = true;
          console.log('Onglets Blazor initialisés avec succès');
    });
    },
    
    // Attendre que Bootstrap soit chargé
    waitForBootstrap: function(callback) {
        let attempts = 0;
        const maxAttempts = 50; // 5 secondes max
        
        const checkBootstrap = () => {
 attempts++;
       
            if (window.bootstrap && window.bootstrap.Tab) {
    callback();
     } else if (attempts < maxAttempts) {
    setTimeout(checkBootstrap, 100);
  } else {
        console.error('Bootstrap non disponible après 5 secondes');
                // Fallback vers une solution pure CSS/JavaScript
      this.setupManualTabs();
            }
        };
     
      checkBootstrap();
    },
    
    // Configurer les onglets avec Bootstrap
    setupTabs: function() {
   const tabButtons = document.querySelectorAll('[data-bs-toggle="tab"]');
    
     console.log(`Configuration de ${tabButtons.length} onglets`);
     
  tabButtons.forEach((button, index) => {
   console.log(`Onglet ${index}: ${button.getAttribute('data-bs-target')}`);
    
     // Nettoyer les anciens listeners
            button.removeEventListener('click', this.handleTabClick);

          // Ajouter le nouveau listener
            button.addEventListener('click', this.handleTabClick.bind(this));
    
      // Initialiser Bootstrap Tab si pas déjà fait
            if (!button._blazorTab) {
      try {
      button._blazorTab = new window.bootstrap.Tab(button);
    } catch (error) {
        console.error('Erreur création Tab Bootstrap:', error);
            }
            }
        });
    
        // Ajouter des listeners pour les événements Bootstrap
   this.setupBootstrapEvents();
    },
    
    // Gérer le clic sur un onglet
    handleTabClick: function(event) {
        event.preventDefault();
        console.log('Clic sur onglet:', event.target.getAttribute('data-bs-target'));
        
        const targetId = event.target.getAttribute('data-bs-target');
      if (targetId) {
            this.showTab(targetId, event.target);
        }
    },
    
    // Afficher un onglet spécifique
    showTab: function(targetId, tabButton = null) {
        console.log('Affichage onglet:', targetId);
        
    // Trouver le bouton si pas fourni
        if (!tabButton) {
            tabButton = document.querySelector(`[data-bs-target="${targetId}"]`);
        }
        
        if (!tabButton) {
            console.error('Bouton onglet non trouvé pour:', targetId);
        return;
        }
        
  // Utiliser Bootstrap si disponible
     if (window.bootstrap && window.bootstrap.Tab && tabButton._blazorTab) {
      try {
        tabButton._blazorTab.show();
           return;
   } catch (error) {
     console.error('Erreur Bootstrap Tab.show():', error);
         }
        }
        
    // Fallback manuel
        this.showTabManually(targetId, tabButton);
    },
    
    // Fallback manuel sans Bootstrap
    showTabManually: function(targetId, tabButton) {
        console.log('Affichage manuel de l\'onglet:', targetId);
    
        // Désactiver tous les onglets
const allTabs = document.querySelectorAll('[data-bs-toggle="tab"]');
     const allPanes = document.querySelectorAll('.tab-pane');
        
 allTabs.forEach(tab => {
tab.classList.remove('active');
 tab.setAttribute('aria-selected', 'false');
        });
        
      allPanes.forEach(pane => {
  pane.classList.remove('show', 'active');
    });
      
        // Activer l'onglet sélectionné
        tabButton.classList.add('active');
        tabButton.setAttribute('aria-selected', 'true');
        
     // Activer le contenu correspondant
        const targetPane = document.querySelector(targetId);
        if (targetPane) {
            targetPane.classList.add('show', 'active');
        }
    },
    
    // Configurer sans Bootstrap (fallback)
    setupManualTabs: function() {
        console.log('Configuration manuelle des onglets (fallback)');
        
        const tabButtons = document.querySelectorAll('[data-bs-toggle="tab"]');
        
        tabButtons.forEach(button => {
            button.removeEventListener('click', this.handleTabClick);
            button.addEventListener('click', this.handleTabClick.bind(this));
        });
    },
    
    // Configurer les événements Bootstrap
    setupBootstrapEvents: function() {
 document.addEventListener('shown.bs.tab', (event) => {
      console.log('Événement shown.bs.tab:', event.target.getAttribute('data-bs-target'));
        });
        
        document.addEventListener('hidden.bs.tab', (event) => {
      console.log('Événement hidden.bs.tab:', event.target.getAttribute('data-bs-target'));
        });
    },
 
    // Nettoyer
    cleanup: function() {
        const tabButtons = document.querySelectorAll('[data-bs-toggle="tab"]');
        tabButtons.forEach(button => {
            button.removeEventListener('click', this.handleTabClick);
            if (button._blazorTab) {
                try {
      button._blazorTab.dispose();
        } catch (error) {
             console.warn('Erreur lors du nettoyage Tab Bootstrap:', error);
  }
    delete button._blazorTab;
       }
        });
    },
    
    // API publique pour activer un onglet
    activate: function(tabId) {
        this.showTab(tabId);
    }
};

// Fonctions globales pour compatibilité
window.initializeBootstrapTabs = function() {
    window.blazorTabs.init();
};

window.activateTab = function(tabId) {
    window.blazorTabs.activate(tabId);
};

// Auto-initialisation
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM chargé - Initialisation des onglets Blazor');
    window.blazorTabs.init();
});

// Réinitialisation après chargement complet
window.addEventListener('load', () => {
    setTimeout(() => {
    console.log('Window load - Réinitialisation des onglets');
        window.blazorTabs.init();
    }, 300);
});