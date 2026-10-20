
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('user_pets', function (Blueprint $table) {
            $table->id();

            $table->unsignedBigInteger('elder_user_id');
            $table->unsignedBigInteger('animals_id');

            $table->string('nickname', 100)->nullable();

            $table->dateTime('created_at')->useCurrent();
            $table->dateTime('updated_at')
                ->useCurrent()
                ->useCurrentOnUpdate();

            // 高齢者ユーザーとの外部キー制約
            $table->foreign('elder_user_id')
                ->references('id')
                ->on('elder_users');

            // ペットの種類との外部キー制約
            $table->foreign('animals_id')
                ->references('id')
                ->on('animals');
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('user_pets');
    }
};
