// ============================================================
// 1. KHỞI TẠO CÁC BIẾN TOÀN CỤC (GLOBAL VARIABLES)
// ============================================================

// Tạo kết nối đến SignalR Hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/priceHub")
    .build();

// Biến lưu trữ danh sách tài sản ĐANG THEO DÕI (của riêng bạn)
let assets = [];

// Biến lưu trữ danh sách TẤT CẢ COIN TRÊN THỊ TRƯỜNG (để làm dropdown gợi ý)
let marketCoins = [];

// Biến lưu trữ đối tượng biểu đồ (để xóa đi vẽ lại)
let myChart = null;

// ============================================================
// 2. KHỞI ĐỘNG ỨNG DỤNG
// ============================================================

// --- SỬA HÀM start() ---
async function start() {
    // 1. Kiểm tra xem đã đăng nhập chưa
    // Nếu chưa (token = null), hàm checkAuth() trong auth.js sẽ tự chuyển hướng về login.html
    const token = checkAuth();
    if (!token) return; // Dừng chạy nếu không có token

    // 2. Hiển thị Email người dùng lên thanh menu
    const userEmail = localStorage.getItem('userEmail');
    if (document.getElementById('userDisplay')) {
        document.getElementById('userDisplay').innerText = `Xin chào, ${userEmail}`;
    }

    try {
        await connection.start();
        console.log("SignalR Connected.");
        await loadMarketData();
        await loadAssets(); // Hàm này cần sửa để gửi Token (xem bên dưới)
    } catch (err) {
        console.error(err);
        setTimeout(start, 5000);
    }
}

// ============================================================
// 3. LOGIC TẢI DỮ LIỆU THỊ TRƯỜNG (CHO DROPDOWN)
// ============================================================

async function loadMarketData() {
    const dataList = document.getElementById('coinList');
    if (!dataList) return;

    try {
        console.log("Đang tải danh sách coin từ API...");

        // Thử gọi API (timeout trong 5 giây để không phải chờ lâu)
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 5000);

        const response = await fetch('https://api.coincap.io/v2/assets?limit=2000', {
            signal: controller.signal
        });
        clearTimeout(timeoutId);

        if (!response.ok) throw new Error("Kết nối API thất bại");

        const data = await response.json();
        marketCoins = data.data;
        console.log(`✅ Đã tải thành công ${marketCoins.length} coin từ API.`);

    } catch (error) {
        console.warn("⚠️ API lỗi hoặc bị chặn, đang dùng danh sách Offline:", error);

        // DANH SÁCH DỰ PHÒNG (Mở rộng thêm nhiều coin phổ biến để bạn test)
        marketCoins = [
            { symbol: "BTC", name: "Bitcoin" },
            { symbol: "ETH", name: "Ethereum" },
            { symbol: "USDT", name: "Tether" },
            { symbol: "BNB", name: "BNB" },
            { symbol: "SOL", name: "Solana" },
            { symbol: "XRP", name: "XRP" },
            { symbol: "USDC", name: "USDC" },
            { symbol: "ADA", name: "Cardano" },
            { symbol: "DOGE", name: "Dogecoin" },
            { symbol: "TRX", name: "TRON" },
            { symbol: "DOT", name: "Polkadot" },
            { symbol: "MATIC", name: "Polygon" },
            { symbol: "LTC", name: "Litecoin" },
            { symbol: "SHIB", name: "Shiba Inu" },
            { symbol: "AVAX", name: "Avalanche" },
            { symbol: "DAI", name: "Dai" },
            { symbol: "WBTC", name: "Wrapped Bitcoin" },
            { symbol: "LINK", name: "Chainlink" },
            { symbol: "ATOM", name: "Cosmos" },
            { symbol: "UNI", name: "Uniswap" }
        ];

        // BỎ ALERT ĐỂ KHÔNG BỊ CHẶN GIAO DIỆN
        // alert("⚠️ Không kết nối được API! Đang dùng chế độ Offline.");
    }

    // --- ĐỔ DỮ LIỆU VÀO DROPDOWN ---
    dataList.innerHTML = '';

    marketCoins.forEach(coin => {
        const option = document.createElement('option');
        // Value hiển thị: "BTC - Bitcoin"
        option.value = `${coin.symbol} - ${coin.name}`;
        dataList.appendChild(option);
    });
}

// ============================================================
// 4. CÁC HÀM XỬ LÝ DỮ LIỆU CHÍNH (CRUD)
// ============================================================

// Hàm tải danh sách tài sản của bạn từ Server
// --- SỬA HÀM loadAssets() ---
// Thêm Token vào Header khi gọi API lấy danh sách
async function loadAssets() {
    const token = localStorage.getItem('authToken'); // Lấy token

    const response = await fetch('/api/assets', {
        method: 'GET',
        headers: {
            // Gửi Token lên Server để chứng minh "Tôi đã đăng nhập"
            'Authorization': `Bearer ${token}`
        }
    });

    if (response.status === 401) {
        // Nếu Server trả về 401 Unauthorized (Token hết hạn hoặc lởm) -> Logout ngay
        logout();
        return;
    }

    assets = await response.json();
    renderTable();
}

// --- LƯU Ý: Sửa tương tự cho các hàm createAsset, deleteAsset, loadChart ---
// Chỉ cần thêm headers: { 'Authorization': `Bearer ${token}`, ... } vào hàm fetch

// Hàm định dạng tiền tệ
function formatMoney(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// === HÀM TẠO MỚI (ĐÃ SỬA ĐỂ XỬ LÝ DROPDOWN) ===
async function createAsset() {
    // Lấy giá trị từ ô input duy nhất (nơi người dùng chọn hoặc gõ)
    const inputValue = document.getElementById('coinInput').value;

    // Kiểm tra rỗng
    if (!inputValue) return alert("Vui lòng chọn hoặc nhập tên coin!");

    // Biến để chứa kết quả sau khi xử lý
    let symbol = "";
    let name = "";

    // Kiểm tra xem người dùng có chọn đúng định dạng "SYMBOL - Name" không
    // Ví dụ: "BTC - Bitcoin" -> Tách ra mảng ["BTC", "Bitcoin"]
    const parts = inputValue.split(' - ');

    if (parts.length >= 2) {
        // Trường hợp 1: Người dùng chọn từ Dropdown (Đúng chuẩn)
        symbol = parts[0].trim().toUpperCase(); // Lấy phần trước dấu gạch ngang
        name = parts[1].trim(); // Lấy phần sau dấu gạch ngang
    } else {
        // Trường hợp 2: Người dùng tự gõ tay (VD: chỉ gõ "DOGE" hoặc "Ethereum")
        // Ta phải tìm trong danh sách marketCoins xem có khớp không
        const userInput = inputValue.toUpperCase().trim();

        const foundCoin = marketCoins.find(c =>
            c.symbol.toUpperCase() === userInput ||
            c.name.toUpperCase() === userInput
        );

        if (foundCoin) {
            // Nếu tìm thấy trong danh sách
            symbol = foundCoin.symbol;
            name = foundCoin.name;
        } else {
            // Trường hợp 3: Không tìm thấy gì cả (Coin lạ), lấy luôn cái họ nhập làm Symbol
            symbol = userInput;
            name = inputValue.trim();
        }
    }

    // Gửi dữ liệu đã xử lý lên Server
    try {
        const response = await fetch('/api/assets', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ symbol, name })
        });

        if (response.ok) {
            // Xóa trắng ô input sau khi thêm
            document.getElementById('coinInput').value = '';
            // Tải lại bảng để hiện coin mới
            loadAssets();
        } else {
            const errorData = await response.json();
            alert("Lỗi: " + (errorData.message || "Không thể thêm coin này"));
        }
    } catch (e) {
        alert("Lỗi kết nối đến Server!");
    }
}

// Hàm xóa tài sản
async function deleteAsset(id, symbol) {
    if (!confirm(`Bạn có chắc muốn xóa ${symbol} không?`)) return;

    try {
        const response = await fetch(`/api/assets/${id}`, { method: 'DELETE' });

        if (response.ok) {
            // Xóa khỏi danh sách local
            assets = assets.filter(a => a.id !== id);
            renderTable();

            // Nếu đang xem biểu đồ coin này thì tắt đi
            if (myChart && myChart.data.datasets[0].label === `Giá ${symbol}`) {
                myChart.destroy();
                myChart = null;
                document.getElementById('chartPlaceholder').style.display = 'block';
            }
        } else {
            alert("Lỗi khi xóa!");
        }
    } catch (e) {
        console.error(e);
        alert("Lỗi kết nối Server.");
    }
}

// ============================================================
// 5. XỬ LÝ SIGNALR (REAL-TIME)
// ============================================================

connection.on("ReceivePriceUpdate", (data) => {
    // Tìm coin trong danh sách hiển thị
    const asset = assets.find(a => a.symbol === data.symbol);

    if (asset) {
        // Cập nhật data local
        asset.currentPrice = data.price;
        asset.lastUpdated = data.timestamp;

        // Tìm dòng trong bảng để update giao diện
        const row = document.getElementById(`row-${asset.symbol}`);
        if (row) {
            const priceCell = row.querySelector('.price-cell');
            priceCell.innerText = formatMoney(asset.currentPrice);

            // Hiệu ứng nhấp nháy xanh
            priceCell.classList.remove('price-up');
            void priceCell.offsetWidth; // Trigger reflow
            priceCell.classList.add('price-up');

            row.querySelector('.time-cell').innerText = new Date(asset.lastUpdated).toLocaleTimeString();
        }

        // Nếu đang mở biểu đồ coin này -> thêm data vào biểu đồ
        if (myChart && myChart.data.datasets[0].label === `Giá ${asset.symbol}`) {
            addDataToChart(myChart, data.timestamp, data.price);
        }
    }
});

// ============================================================
// 6. VẼ GIAO DIỆN (RENDER UI)
// ============================================================

function renderTable() {
    const tbody = document.getElementById('assetTableBody');
    tbody.innerHTML = '';

    assets.forEach(asset => {
        // URL Logo từ CoinCap
        const logoUrl = `https://assets.coincap.io/assets/icons/${asset.symbol.toLowerCase()}@2x.png`;

        const tr = document.createElement('tr');
        tr.id = `row-${asset.symbol}`;
        tr.innerHTML = `
            <td class="ps-4">
                <div class="d-flex align-items-center">
                    <img src="${logoUrl}" class="coin-logo" onerror="this.src='https://via.placeholder.com/32?text=${asset.symbol[0]}'">
                    <div>
                        <div class="fw-bold">${asset.symbol}</div>
                        <small class="text-muted">${asset.name}</small>
                    </div>
                </div>
            </td>
            <td class="price-cell fw-bold text-success fs-5">${formatMoney(asset.currentPrice)}</td>
            <td class="time-cell text-muted small">${new Date(asset.lastUpdated).toLocaleTimeString()}</td>
            <td class="text-end pe-4">
                <button class="btn btn-icon btn-outline-primary me-1" onclick="loadChart(${asset.id}, '${asset.symbol}')" title="Xem biểu đồ">
                    <i class="fa-solid fa-chart-line"></i>
                </button>
                <button class="btn btn-icon btn-outline-danger" onclick="deleteAsset(${asset.id}, '${asset.symbol}')" title="Xóa">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// ============================================================
// 7. XỬ LÝ BIỂU ĐỒ (CHART.JS)
// ============================================================

async function loadChart(assetId, symbol) {
    const response = await fetch(`/api/assets/${assetId}/history`);
    const historyData = await response.json();

    const labels = historyData.map(h => new Date(h.timestamp).toLocaleTimeString()).reverse();
    const prices = historyData.map(h => h.price).reverse();

    document.getElementById('chartPlaceholder').style.display = 'none';
    const ctx = document.getElementById('priceChart').getContext('2d');

    if (myChart) myChart.destroy();

    myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: `Giá ${symbol}`,
                data: prices,
                borderColor: '#ffc107',
                backgroundColor: 'rgba(255, 193, 7, 0.1)',
                borderWidth: 2,
                tension: 0.4,
                pointRadius: 0,
                fill: true
            }]
        },
        options: {
            responsive: true,
            interaction: { intersect: false },
            plugins: { legend: { display: false } },
            scales: {
                x: { display: false },
                y: {
                    grid: { color: 'rgba(255,255,255,0.05)' },
                    ticks: { color: '#888' }
                }
            }
        }
    });
}

function addDataToChart(chart, timestamp, price) {
    chart.data.labels.push(new Date(timestamp).toLocaleTimeString());
    chart.data.datasets[0].data.push(price);

    if (chart.data.labels.length > 50) {
        chart.data.labels.shift();
        chart.data.datasets[0].data.shift();
    }
    chart.update('none');
}

// Chạy ứng dụng
start();