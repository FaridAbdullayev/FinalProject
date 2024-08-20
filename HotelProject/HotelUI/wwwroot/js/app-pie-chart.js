//document.addEventListener('DOMContentLoaded', async () => {
//    try {
//        // API'den veri çekme
//        const response = await fetch('https://localhost:7119/api/branches/incomes');

//        console.log(response)
//        if (!response.ok) throw new Error('Network response was not ok');
//        const data = await response.json();

//        // Verileri hazırlama
//        const branchNames = data.map(branch => branch.branchName);
//        const branchIncomes = data.map(branch => branch.income);

//        // Chart.js ile grafiği oluşturma
//        const ctx = document.getElementById('branch-income-chart').getContext('2d');
//        new Chart(ctx, {
//            type: 'pie',
//            data: {
//                labels: branchNames,
//                datasets: [{
//                    label: 'Şube Gelirleri',
//                    data: branchIncomes,
//                    backgroundColor: [
//                        'rgba(255, 99, 132, 0.2)',
//                        'rgba(54, 162, 235, 0.2)',
//                        'rgba(255, 206, 86, 0.2)',
//                        'rgba(75, 192, 192, 0.2)',
//                        'rgba(153, 102, 255, 0.2)',
//                        'rgba(255, 159, 64, 0.2)'
//                    ],
//                    borderColor: [
//                        'rgba(255, 99, 132, 1)',
//                        'rgba(54, 162, 235, 1)',
//                        'rgba(255, 206, 86, 1)',
//                        'rgba(75, 192, 192, 1)',
//                        'rgba(153, 102, 255, 1)',
//                        'rgba(255, 159, 64, 1)'
//                    ],
//                    borderWidth: 1
//                }]
//            },
//            options: {
//                responsive: true,
//                plugins: {
//                    legend: {
//                        position: 'top',
//                    },
//                    tooltip: {
//                        callbacks: {
//                            label: function (tooltipItem) {
//                                return `${tooltipItem.label}: ${tooltipItem.raw.toFixed(2)} TL`;
//                            }
//                        }
//                    }
//                }
//            }
//        });
//    } catch (error) {
//        console.error('Error fetching branch incomes:', error);
//    }
//});


//$(document).ready(function () {
//    // API'den veri çek
//    $.ajax({
//        url: 'http://localhost:7119/api/branches/incomes',
//        method: 'GET',
//        success: function (data) {
//            // Veriyi işleyerek chart için uygun formatta hazırla
//            var branchNames = data.map(branch => branch.branchName);
//            var branchIncomes = data.map(branch => branch.income);

//            // Chart.js ile pie chart oluştur
//            var ctx = $('#branch-income-chart').get(0).getContext('2d');
//            var branchIncomeChart = new Chart(ctx, {
//                type: 'pie',
//                data: {
//                    labels: branchNames,
//                    datasets: [{
//                        label: 'Branch Incomes',
//                        data: branchIncomes,
//                        backgroundColor: [
//                            '#745af2',
//                            '#5cd069',
//                            '#fecb01',
//                            '#4bc0c0',
//                            '#ff6384',
//                            '#36a2eb'
//                        ],
//                        borderColor: '#fff',
//                        borderWidth: 1
//                    }]
//                },
//                options: {
//                    responsive: true,
//                    plugins: {
//                        legend: {
//                            position: 'top',
//                            labels: {
//                                usePointStyle: true
//                            }
//                        },
//                        tooltip: {
//                            callbacks: {
//                                label: function (tooltipItem) {
//                                    return `${tooltipItem.label}: ${tooltipItem.raw.toFixed(2)} TL`;
//                                }
//                            }
//                        }
//                    }
//                }
//            });

//            // Eğer legend eklemek isterseniz, generateLegend() fonksiyonunu kullanabilirsiniz
//            $('#branch-income-legend').html(branchIncomeChart.generateLegend());
//        },
//        error: function (xhr, status, error) {
//            console.error('Error fetching branch incomes:', error);
//        }
//    });
//});
