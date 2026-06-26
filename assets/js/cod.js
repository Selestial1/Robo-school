const TRAINERS = {};

function highlightActiveNav() {
    const page = document.body.dataset.page;
    if (!page) return;

    document.querySelectorAll('[data-nav]').forEach((link) => {
        link.classList.toggle('is-active', link.dataset.nav === page);
    });
}

async function loadTrainersFromApi() {
    try {
        const res = await fetch('/api/trainers');
        if (!res.ok) return;
        const trainers = await res.json();
        trainers.forEach((t) => {
            TRAINERS[t.slug] = {
                name: t.name,
                role: t.role,
                photo: t.photoUrl,
                text: t.bio,
            };
        });
    } catch {
        Object.assign(TRAINERS, {
            irina: {
                name: 'Ирина Лайм',
                role: 'преподаватель по робототехнике',
                photo: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=560&h=400&fit=crop',
                text: 'Ирина — педагог с 8-летним опытом преподавания робототехники в начальной школе.',
            },
            marina: {
                name: 'Марина Орлова',
                role: 'преподаватель по робототехнике',
                photo: 'https://images.unsplash.com/photo-1580489944761-15a19d654956?w=560&h=400&fit=crop',
                text: 'Марина специализируется на проектной деятельности и соревнованиях по робототехнике.',
            },
            maxim: {
                name: 'Максим Петров',
                role: 'преподаватель по программированию',
                photo: 'https://images.unsplash.com/photo-1560250097-0b93528c311a?w=560&h=400&fit=crop',
                text: 'Максим обучает Scratch, Python и основам алгоритмизации для детей 6–12 лет.',
            },
            konstantin: {
                name: 'Константин Назаров',
                role: 'преподаватель по робототехнике',
                photo: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=560&h=400&fit=crop',
                text: 'Константин — инженер-педагог с опытом работы в R:ED LAB.',
            },
            liza: {
                name: 'Лиза Весенняя',
                role: 'преподаватель по программированию',
                photo: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=560&h=400&fit=crop',
                text: 'Лиза ведёт курсы по визуальному программированию и цифровой грамотности.',
            },
        });
    }
}

function formatPrice(price) {
    return `${Number(price).toLocaleString('ru-RU')} ₽`;
}

async function loadPackagesFromApi() {
    const grid = document.getElementById('packages-grid');
    const pkgSelect = document.getElementById('package-select');
    if (!grid && !pkgSelect) return;

    try {
        const res = await fetch('/api/packages');
        if (!res.ok) return;
        const packages = await res.json();
        if (!packages.length) return;

        if (grid) {
            const cardClasses = ['cart', 'cart1', 'cart2'];
            grid.innerHTML = packages.map((pkg, index) => {
                const cardClass = cardClasses[index % cardClasses.length];
                return `
                <article class="${cardClass}">
                    <h3 class="clt">– ${pkg.name.replace(/^[\s\-–]+|[\s\-–]+$/g, '')} –</h3>
                    <p class="clt1">${formatPrice(pkg.price)}</p>
                    <p class="clt2">${pkg.description}</p>
                    <button type="button" class="button1 btn-primary btn-package" data-package="${pkg.code}">
                        <span class="text2">Записаться</span>
                    </button>
                </article>
            `;
            }).join('');

            bindPackageButtons();
        }

        if (pkgSelect) {
            const current = (pkgSelect.value || new URLSearchParams(window.location.search).get('package') || '').toUpperCase();
            pkgSelect.innerHTML = `
                <option value="">— Выберите пакет —</option>
                ${packages.map((pkg) => `
                    <option value="${pkg.code}">${pkg.code} — ${pkg.description} (${formatPrice(pkg.price)})</option>
                `).join('')}
            `;
            if (current) syncPackageSelection(current);
        }
    } catch {
        bindPackageButtons();
    }
}

const menuToggle = document.getElementById('menu-toggle');
const mobileNav = document.getElementById('mobile-nav');
const trainerModal = document.getElementById('trainer-modal');
const modalClose = document.getElementById('modal-close');
const registerForm = document.getElementById('register-form');
const packageField = document.getElementById('package-field');
const selectedPackageEl = document.getElementById('selected-package');
const formMessage = document.getElementById('form-message');

function closeMobileMenu() {
    if (!mobileNav || !menuToggle) return;
    mobileNav.classList.remove('is-open');
    mobileNav.setAttribute('aria-hidden', 'true');
    menuToggle.setAttribute('aria-expanded', 'false');
    menuToggle.querySelector('.icon-menu').hidden = false;
    menuToggle.querySelector('.icon-close').hidden = true;
    document.body.style.overflow = '';
}

function openMobileMenu() {
    if (!mobileNav || !menuToggle) return;
    mobileNav.classList.add('is-open');
    mobileNav.setAttribute('aria-hidden', 'false');
    menuToggle.setAttribute('aria-expanded', 'true');
    menuToggle.querySelector('.icon-menu').hidden = true;
    menuToggle.querySelector('.icon-close').hidden = false;
    document.body.style.overflow = 'hidden';
}

menuToggle?.addEventListener('click', () => {
    if (mobileNav.classList.contains('is-open')) {
        closeMobileMenu();
    } else {
        openMobileMenu();
    }
});

document.querySelectorAll('.mobile-nav-link').forEach((link) => {
    link.addEventListener('click', closeMobileMenu);
});

document.querySelectorAll('[data-scroll]').forEach((btn) => {
    btn.addEventListener('click', () => {
        const target = document.querySelector(btn.dataset.scroll);
        target?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
});

document.querySelectorAll('.nav-link[href^="#"]').forEach((link) => {
    link.addEventListener('click', (e) => {
        e.preventDefault();
        const target = document.querySelector(link.getAttribute('href'));
        target?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
});

function openTrainerModal(id) {
    const trainer = TRAINERS[id];
    if (!trainer || !trainerModal) return;

    document.getElementById('modal-photo').src = trainer.photo;
    document.getElementById('modal-photo').alt = trainer.name;
    document.getElementById('modal-title').textContent = trainer.name;
    document.getElementById('modal-role').textContent = trainer.role;
    document.getElementById('modal-text').textContent = trainer.text;

    trainerModal.classList.add('is-open');
    trainerModal.setAttribute('aria-hidden', 'false');
    document.body.style.overflow = 'hidden';
}

function closeTrainerModal() {
    if (!trainerModal) return;
    trainerModal.classList.remove('is-open');
    trainerModal.setAttribute('aria-hidden', 'true');
    document.body.style.overflow = '';
}

function bindTrainerButtons() {
    document.querySelectorAll('.btn-trainer').forEach((btn) => {
        btn.addEventListener('click', () => openTrainerModal(btn.dataset.trainer));
    });
}

modalClose?.addEventListener('click', closeTrainerModal);

trainerModal?.addEventListener('click', (e) => {
    if (e.target === trainerModal) closeTrainerModal();
});

document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        closeTrainerModal();
        closeMobileMenu();
    }
});

function selectPackage(pkg) {
    if (!pkg) return;
    window.location.href = `apply.html?package=${encodeURIComponent(pkg)}`;
}

function syncPackageSelection(pkg) {
    if (!pkg) return;
    if (packageField) packageField.value = pkg;
    const pkgSelect = document.getElementById('package-select');
    if (pkgSelect) pkgSelect.value = pkg;
    if (selectedPackageEl) {
        selectedPackageEl.hidden = false;
        selectedPackageEl.textContent = `Выбран пакет: ${pkg}`;
    }
}

function bindPackageSelect() {
    const pkgSelect = document.getElementById('package-select');
    if (!pkgSelect) return;

    pkgSelect.addEventListener('change', () => {
        const pkg = pkgSelect.value.trim().toUpperCase();
        if (packageField) packageField.value = pkg;
        if (selectedPackageEl) {
            if (pkg) {
                selectedPackageEl.hidden = false;
                selectedPackageEl.textContent = `Выбран пакет: ${pkg}`;
            } else {
                selectedPackageEl.hidden = true;
                selectedPackageEl.textContent = '';
            }
        }
    });
}

function bindPackageButtons() {
    document.querySelectorAll('.btn-package').forEach((btn) => {
        btn.addEventListener('click', () => selectPackage(btn.dataset.package));
    });
}

function applyPackageFromUrl() {
    const params = new URLSearchParams(window.location.search);
    const pkg = params.get('package')?.trim().toUpperCase();
    if (!pkg) return;
    syncPackageSelection(pkg);
}

registerForm?.addEventListener('submit', async (e) => {
    e.preventDefault();
    formMessage.textContent = '';
    formMessage.className = 'form-message';

    const formData = new FormData(registerForm);
    const name = formData.get('name')?.toString().trim();
    const phone = formData.get('phone')?.toString().trim();
    const email = formData.get('email')?.toString().trim();
    const pkg = formData.get('package')?.toString().trim()
        || document.getElementById('package-select')?.value?.trim()
        || null;

    let valid = true;

    registerForm.querySelectorAll('.input').forEach((input) => {
        input.classList.remove('invalid');
        if (!input.checkValidity()) {
            input.classList.add('invalid');
            valid = false;
        }
    });

    if (!valid) {
        formMessage.textContent = 'Пожалуйста, заполните все поля корректно.';
        formMessage.classList.add('error');
        return;
    }

    const submitBtn = registerForm.querySelector('.btn-submit');
    const originalText = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="foot3">Отправка...</span>';

    try {
        const res = await fetch('/api/applications', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                name,
                phone,
                email,
                package: pkg || null,
            }),
        });

        const data = await res.json().catch(() => ({}));

        if (!res.ok) {
            formMessage.textContent = data.error || 'Не удалось отправить заявку. Запустите сервер через start.bat.';
            formMessage.classList.add('error');
            return;
        }

        const emailNote = data.emailSent
            ? ` На почту ${email} отправлено письмо с подтверждением.`
            : '';
        formMessage.textContent = `Спасибо, ${name}! Заявка №${data.id} принята. Мы свяжемся с вами в ближайшие несколько дней.${emailNote}`;
        formMessage.classList.add('success');
        registerForm.reset();
        if (packageField) packageField.value = '';
        const pkgSelect = document.getElementById('package-select');
        if (pkgSelect) pkgSelect.value = '';
        if (selectedPackageEl) {
            selectedPackageEl.hidden = true;
            selectedPackageEl.textContent = '';
        }
    } catch {
        formMessage.textContent = 'Сервер недоступен. Запустите start.bat и откройте http://localhost:5080';
        formMessage.classList.add('error');
    } finally {
        submitBtn.disabled = false;
        submitBtn.innerHTML = originalText;
    }
});

highlightActiveNav();
loadTrainersFromApi().then(() => bindTrainerButtons());
bindPackageButtons();
bindPackageSelect();
loadPackagesFromApi().then(() => applyPackageFromUrl());

if (document.querySelector('.trainers-swiper')) {
    new Swiper('.trainers-swiper', {
        direction: 'horizontal',
        loop: true,
        slidesPerView: 1,
        spaceBetween: 24,
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            640: { slidesPerView: 2 },
            1024: { slidesPerView: 3 },
            1280: { slidesPerView: 4 },
        },
    });
}
