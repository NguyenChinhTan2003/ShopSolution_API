<script>
      $(document).ready(function () {
     // Khai báo các biến lưu trữ
     var selectedTinh = { id: 0, name: "" };
     var selectedQuan = { id: 0, name: "" };
     var selectedPhuong = { id: 0, name: "" };

     // Lấy tỉnh thành
     $.getJSON('https://esgoo.net/api-tinhthanh/1/0.htm', function (data_tinh) {
         if (data_tinh.error == 0) {
             $.each(data_tinh.data, function (key_tinh, val_tinh) {
                 $("#tinh").append('<option value="' + val_tinh.id + '">' + val_tinh.full_name + '</option>');
             });

             $("#tinh").change(function (e) {
                 var idtinh = $(this).val();
                 selectedTinh.id = idtinh;
                 selectedTinh.name = $("#tinh option:selected").text();
                 
                 //Lấy quận huyện
                 $.getJSON('https://esgoo.net/api-tinhthanh/2/' + idtinh + '.htm', function (data_quan) {
                     if (data_quan.error == 0) {
                         $("#quan").html('<option value="0">Quận Huyện</option>');
                         $("#phuong").html('<option value="0">Phường Xã</option>');
                         $.each(data_quan.data, function (key_quan, val_quan) {
                             $("#quan").append('<option value="' + val_quan.id + '">' + val_quan.full_name + '</option>');
                         });
                     }
                     updateCombinedAddress();
                 });
             });
         }
     });

     // Lấy Quận/Huyện
     $("#quan").change(function (e) {
         var idquan = $(this).val();
         selectedQuan.id = idquan;
         selectedQuan.name = $("#quan option:selected").text();

         $.getJSON('https://esgoo.net/api-tinhthanh/3/' + idquan + '.htm', function (data_phuong) {
             if (data_phuong.error == 0) {
                 $("#phuong").html('<option value="0">Phường Xã</option>');
                 $.each(data_phuong.data, function (key_phuong, val_phuong) {
                     $("#phuong").append('<option value="' + val_phuong.id + '">' + val_phuong.full_name + '</option>');
                 });
             }
             updateCombinedAddress();
         });
     });

     //Lấy Phường/Xã
     $("#phuong").change(function (e) {
         selectedPhuong.id = $(this).val();
         selectedPhuong.name = $("#phuong option:selected").text();
         updateCombinedAddress();
     });

     $('#addressDetail').on('input', function () {
         updateCombinedAddress();
     });

       // Hàm cập nhật giá trị input ẩn (địa chỉ gộp)
     function updateCombinedAddress() {
         var combinedAddress = "";
         // Lấy địa chỉ chi tiết
         var addressDetail = $('#addressDetail').val();

         if (addressDetail) {
             combinedAddress = addressDetail.trim();
         }

         //Thêm xã vào
         if (selectedPhuong.name) {
             if (combinedAddress)
                 combinedAddress += ", ";
             combinedAddress += selectedPhuong.name;
         }
         //Thêm huyện vào
         if (selectedQuan.name) {
             if (combinedAddress)
                 combinedAddress += ", ";
             combinedAddress += selectedQuan.name;
         }

         //Thêm tỉnh vào
         if (selectedTinh.name) {
             if (combinedAddress)
                 combinedAddress += ", ";
             combinedAddress += selectedTinh.name;
         }

         $('#codAddress').val(combinedAddress.trim(', '));
         $('#vnpayAddress').val(combinedAddress.trim(', '));
     }

      // Bắt sự kiện khi click vào nút tính phí ship
      $('#calculateShipping').click(function (e) {
         var city = selectedTinh.name;
         var district = selectedQuan.name;
         var ward = selectedPhuong.name;

         // Kiểm tra xem địa chỉ đã được chọn hay chưa
          if (!city || !district || !ward) {
             $('#shippingCostDisplay').text("Vui lòng chọn đầy đủ địa chỉ trước khi tính phí!");
              return;
          }

         // Gửi request lên controller
         $.ajax({
              type: "POST",
              url: "/Cart/CalculateShippingCost",
             contentType: "application/json; charset=utf-8",
             data: JSON.stringify({ city: city, district: district, ward: ward }),
             success: function (response) {
                 if (response && response.success) {
                      //Hiển thị phí ship
                    $('#shippingCostDisplay').text("Phí vận chuyển: " + response.price.toLocaleString('vi-VN') + " VNĐ");

                } else {
                      // Show error message nếu server trả về lỗi hoặc response không đúng định dạng
                      $('#shippingCostDisplay').text("Không thể tính phí ship. Vui lòng thử lại!");
                 }
              },
              error: function (xhr, status, error) {
                  console.error("Error Response:", xhr, status, error);
                 $('#shippingCostDisplay').text("Đã có lỗi xảy ra. Vui lòng thử lại!");
             }
         });
      });


     // Lắng nghe sự kiện submit của form COD
     $('#codForm').submit(function (event) {
         // Lấy giá trị từ input email và phone
         var email = $('#email').val();
         var phone = $('#phone').val();

         // Thêm email và phone vào form COD
         $(this).append('<input type="hidden" name="email" value="' + email + '">');
         $(this).append('<input type="hidden" name="phone" value="' + phone + '">');
     });

     // Lắng nghe sự kiện submit của form VNPay
     $('#vnpayForm').submit(function (event) {
         // Lấy giá trị từ input email và phone
         var email = $('#email').val();
         var phone = $('#phone').val();

         // Thêm email và phone vào form VNPay
         $(this).append('<input type="hidden" name="email" value="' + email + '">');
         $(this).append('<input type="hidden" name="phone" value="' + phone + '">');
      });
 });
</script >